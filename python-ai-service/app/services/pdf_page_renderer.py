
import re
from pathlib import Path

import pymupdf


DEFAULT_RENDER_DIR = Path("storage/rendered-pages")
DEFAULT_DPI = 150


def render_pdf_pages_to_png(
    file_bytes: bytes,
    drawing_id: str,
    output_dir: Path = DEFAULT_RENDER_DIR,
    dpi: int = DEFAULT_DPI
) -> list[dict]:
    """
    Render every PDF page to PNG and return image metadata.
    """

    if not re.fullmatch(r"[A-Za-z0-9_-]+", drawing_id):
        raise ValueError(
            "drawing_id may only contain letters, numbers, "
            "hyphens and underscores"
        )

    if dpi <= 0 or dpi > 600:
        raise ValueError("dpi must be between 1 and 600")

    rendered_pages = []
    drawing_dir = output_dir / drawing_id

    drawing_dir.mkdir(parents=True, exist_ok=True)

    with pymupdf.open(stream=file_bytes, filetype="pdf") as document:

        if document.is_encrypted:
            raise ValueError("Encrypted PDF is not supported")

        if len(document) == 0:
            raise ValueError("PDF contains no pages")

        if len(document) > 100:
            raise ValueError("PDF exceeds the maximum of 100 pages")

        for page_index, page in enumerate(document):

            page_number = page_index + 1

            pixmap = page.get_pixmap(
                dpi=dpi,
                colorspace=pymupdf.csRGB,
                alpha=False
            )

            file_name = f"page-{page_number:03d}.png"
            file_path = drawing_dir / file_name

            pixmap.save(str(file_path))

            relative_path = file_path.relative_to(output_dir)

            rendered_pages.append({
                "pageNumber": page_number,
                "fileName": file_name,
                "relativePath": relative_path.as_posix(),
                "width": pixmap.width,
                "height": pixmap.height,
                "dpi": dpi,
                "format": "png",
                "sizeBytes": file_path.stat().st_size
            })

    return rendered_pages
