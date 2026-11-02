import fitz

from app.services.pdf_text_extractor import extract_text_blocks

POINTS_TO_MM = 25.4 / 72


def rect_to_dict(rect: fitz.Rect) -> dict:
    return {
        "x0": round(rect.x0, 2),
        "y0": round(rect.y0, 2),
        "x1": round(rect.x1, 2),
        "y1": round(rect.y1, 2)
    }

def calculate_page_statistics(
    extracted_text: str,
    text_blocks: list[dict],
    image_block_count: int,
) -> dict:
    text = extracted_text or ""

    return {
        "textBlockCount": len(text_blocks),
        "textSpanCount": sum(
            len(block.get("spans", []))
            for block in text_blocks
        ),
        "characterCount": len(text),
        "wordCount": len(text.split()),
        "imageBlockCount": image_block_count,
        "hasText": bool(text.strip()),
    }

def process_pdf(file_bytes: bytes) -> list[dict]:
    pages = []

    with fitz.open(
        stream=file_bytes,
        filetype="pdf"
    ) as document:

        if document.is_encrypted:
            raise ValueError("Encrypted PDF is not supported.")

        if len(document) == 0:
            raise ValueError("PDF contains no pages.")

        if len(document) > 100:
            raise ValueError("PDF exceeds the 100-page limit.")

        for index, page in enumerate(document):

            # Effective page dimensions after rotation
            page_rect = page.rect

            width = page_rect.width
            height = page_rect.height

            width_mm = width * POINTS_TO_MM
            height_mm = height * POINTS_TO_MM

            # Extract text and text blocks
            extracted_text = page.get_text("text").strip()
            text_blocks = extract_text_blocks(page)

            # Get image block count
            page_dict = page.get_text("dict")

            image_block_count = sum(
                1
                for block in page_dict.get("blocks", [])
                if block.get("type") == 1
            )

            # Calculate page statistics
            statistics = calculate_page_statistics(
                extracted_text=extracted_text,
                text_blocks=text_blocks,
                image_block_count=image_block_count
            )

            pages.append({
                "pageNumber": index + 1,

                "width": round(width, 2),
                "height": round(height, 2),

                "widthMm": round(width_mm, 2),
                "heightMm": round(height_mm, 2),

                "rotation": page.rotation,

                "mediaBox": rect_to_dict(page.mediabox),
                "cropBox": rect_to_dict(page.cropbox),

                "extractedText": extracted_text,
                "textBlocks": text_blocks,

                "statistics": statistics
            })

    return pages