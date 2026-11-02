using System.Net.Http.Headers;
using System.Text.Json;
using System.Net.Http.Json;
using DrawingAnalyzer.Application.DTOs;
using DrawingAnalyzer.Application.Interfaces;

namespace DrawingAnalyzer.Infrastructure.AI;

public sealed class PythonAiDrawingAnalyzer(
    HttpClient httpClient) : IAiDrawingAnalyzer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<DrawingAnalysisResult> AnalyzeAsync(
        string pdfFilePath,
        Guid drawingId,
        Guid processingJobId,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(pdfFilePath))
        {
            throw new FileNotFoundException(
                "PDF file was not found.",
                pdfFilePath);
        }

        await using var fileStream = File.OpenRead(pdfFilePath);

        using var form = new MultipartFormDataContent();

        using var fileContent = new StreamContent(fileStream);

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue("application/pdf");

        form.Add(
            fileContent,
            "file",
            Path.GetFileName(pdfFilePath));

        form.Add(
            new StringContent(drawingId.ToString()),
            "drawingId");

        form.Add(
            new StringContent(processingJobId.ToString()),
            "processingJobId");

        using var response = await httpClient.PostAsync(
            "api/v1/analyze",
            form,
            cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Python AI service returned HTTP " +
                $"{(int)response.StatusCode}: {responseBody}",
                null,
                response.StatusCode);
        }

        var result = await response.Content
            .ReadFromJsonAsync<DrawingAnalysisResult>(
                cancellationToken: cancellationToken);

        if (result is null)
        {
            throw new InvalidOperationException(
                "Python AI service returned an empty response.");
        }

        return result;
    }
}