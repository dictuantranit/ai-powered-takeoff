
from app.schemas.analysis import (
    DocumentClassification,
    PageClassification,
)


def classify_page(page: dict) -> PageClassification:
    page_number = page["pageNumber"]
    statistics = page.get("statistics", {})

    text_block_count = statistics.get("textBlockCount", 0)
    character_count = statistics.get("characterCount", 0)
    image_block_count = statistics.get("imageBlockCount", 0)

    has_text = character_count > 0 and text_block_count > 0
    has_images = image_block_count > 0

    warnings: list[str] = []

    if has_text and has_images:
        classification = "mixed"
        confidence = 0.90

    elif has_text:
        classification = "text_based"
        confidence = 0.85

    elif has_images:
        classification = "scanned"
        confidence = 0.70
        warnings.append(
            "Image-based page detected; OCR may be required."
        )

    else:
        classification = "unknown"
        confidence = 0.40
        warnings.append(
            "Insufficient text and image evidence to classify this page."
        )

    return PageClassification(
        pageNumber=page_number,
        classification=classification,
        confidence=confidence,
        warnings=warnings,
    )


def classify_document(
    pages: list[dict],
) -> DocumentClassification:
    if not pages:
        return DocumentClassification(
            classification="unknown",
            confidence=0.0,
            warnings=["Document contains no pages to classify."],
        )

    page_classifications = [
        classify_page(page)
        for page in pages
    ]

    types = {
        result.classification
        for result in page_classifications
    }

    if types == {"text_based"}:
        document_type = "text_based"

    elif types == {"scanned"}:
        document_type = "scanned"

    elif types == {"unknown"}:
        document_type = "unknown"

    else:
        # Different page types, or a combination of known
        # and unknown pages, requires further inspection.
        document_type = "mixed"

    confidence = round(
        sum(item.confidence for item in page_classifications)
        / len(page_classifications),
        2,
    )

    warnings = [
        warning
        for item in page_classifications
        for warning in item.warnings
    ]

    return DocumentClassification(
        classification=document_type,
        confidence=confidence,
        pageClassifications=page_classifications,
        warnings=warnings,
    )
