
import time

from app.schemas.analysis import DrawingAnalysisResponse
from app.services.pdf_processor import process_pdf
from app.services.pdf_metadata_service import extract_pdf_metadata
from app.services.pdf_image_extractor import extract_embedded_images
from app.services.image_storage_service import save_extracted_images
from app.services.pdf_page_renderer import render_pdf_pages_to_png
from app.services.pdf_classifier import classify_document


def analyze_drawing(
    file_bytes: bytes,
    drawing_id: str,
    processing_job_id: str,
) -> DrawingAnalysisResponse:
    started_at = time.perf_counter()

    # 1. Extract PDF document metadata.
    metadata = extract_pdf_metadata(file_bytes)

    # 2. Extract page-level information and text.
    pages = process_pdf(file_bytes)

    # Classify PDF document based on extracted page information.
    classification = classify_document(pages)

    # 3. Extract embedded images from the PDF.
    embedded_images = extract_embedded_images(file_bytes)

    # 4. Save extracted images and collect their metadata.
    extracted_images = save_extracted_images(
        images=embedded_images,
        drawing_id=drawing_id,
    )

    # 5. Render PDF pages to PNG files.
    rendered_pages = render_pdf_pages_to_png(
        file_bytes=file_bytes,
        drawing_id=drawing_id,
    )

    # 6. Calculate total processing time.
    elapsed_ms = int((time.perf_counter() - started_at) * 1000)

    # 7. Build the final response for the API.
    return DrawingAnalysisResponse(
        drawingId=drawing_id,
        processingJobId=processing_job_id,
        status="completed",
        analysisMode="metadata-text-image-extraction-and-page-rendering",
        documentMetadata=metadata,
        classification=classification,
        totalPages=len(pages),
        items=[],
        pages=pages,
        extractedImages=extracted_images,
        renderedPages=rendered_pages,
        processingTimeMs=elapsed_ms,
    )
  