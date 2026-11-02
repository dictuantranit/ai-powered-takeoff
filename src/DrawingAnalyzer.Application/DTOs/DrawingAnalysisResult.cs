namespace DrawingAnalyzer.Application.DTOs;

public class DrawingAnalysisResult
{
    public string DrawingId { get; set; } = string.Empty;
    public string ProcessingJobId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AnalysisMode { get; set; } = string.Empty;

    public DocumentMetadata DocumentMetadata { get; set; } = new();
    public DocumentClassification Classification { get; set; } = new();

    public int TotalPages { get; set; }

    public List<TakeoffItemResult> Items { get; set; } = [];
    public List<PageAnalysis> Pages { get; set; } = [];
    public List<ExtractedImage> ExtractedImages { get; set; } = [];
    public List<RenderedPage> RenderedPages { get; set; } = [];

    public int ProcessingTimeMs { get; set; }
}

public class DocumentMetadata
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Subject { get; set; }
    public string? Keywords { get; set; }
    public string? Creator { get; set; }
    public string? Producer { get; set; }
    public string? CreationDate { get; set; }
    public string? ModificationDate { get; set; }

    public int PageCount { get; set; }
}

public class DocumentClassification
{
    public string Classification { get; set; } = string.Empty;
    public double Confidence { get; set; }

    public List<PageClassification> PageClassifications { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
}

public class PageClassification
{
    public int PageNumber { get; set; }
    public string Classification { get; set; } = string.Empty;
    public double Confidence { get; set; }

    public List<string> Warnings { get; set; } = [];
}

public class TakeoffItemResult
{
    public string ObjectType { get; set; } = string.Empty;
    public string ObjectName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
    public decimal? Confidence { get; set; }
    public int? PageNumber { get; set; }
}

public class PageAnalysis
{
    public int PageNumber { get; set; }

    public double Width { get; set; }
    public double Height { get; set; }

    public double WidthMm { get; set; }
    public double HeightMm { get; set; }

    public int Rotation { get; set; }

    public PageBox MediaBox { get; set; } = new();
    public PageBox CropBox { get; set; } = new();

    public string ExtractedText { get; set; } = string.Empty;

    public List<TextBlock> TextBlocks { get; set; } = [];
    public PageStatistics Statistics { get; set; } = new();
}

public class PageBox
{
    public double X0 { get; set; }
    public double Y0 { get; set; }
    public double X1 { get; set; }
    public double Y1 { get; set; }
}

public class TextBlock
{
    public int BlockNumber { get; set; }
    public string Text { get; set; } = string.Empty;

    public PageBox Bbox { get; set; } = new();

    public List<TextSpan> Spans { get; set; } = [];
}

public class TextSpan
{
    public string Text { get; set; } = string.Empty;
    public string Font { get; set; } = string.Empty;

    public double FontSize { get; set; }

    public int Flags { get; set; }
    public int Color { get; set; }

    public PageBox Bbox { get; set; } = new();
}

public class PageStatistics
{
    public int TextBlockCount { get; set; }
    public int TextSpanCount { get; set; }
    public int CharacterCount { get; set; }
    public int WordCount { get; set; }
    public int ImageBlockCount { get; set; }

    public bool HasText { get; set; }
}

public class ExtractedImage
{
    public int PageNumber { get; set; }
    public int ImageIndex { get; set; }
    public int Xref { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;

    public int Width { get; set; }
    public int Height { get; set; }
    public string Extension { get; set; } = string.Empty;

    public int? Colorspace { get; set; }
    public int? BitsPerComponent { get; set; }

    public int SizeBytes { get; set; }
    public int Smask { get; set; }
}

public class RenderedPage
{
    public int PageNumber { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;

    public int Width { get; set; }
    public int Height { get; set; }

    public int Dpi { get; set; }
    public string Format { get; set; } = string.Empty;

    public int SizeBytes { get; set; }
}