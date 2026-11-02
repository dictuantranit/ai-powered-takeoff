
from pathlib import Path
import pymupdf

from app.services.pdf_page_renderer import render_pdf_pages_to_png


pdf_path = Path("sample.pdf")
drawing_id = "drawing-001"
dpi = 150

if not pdf_path.exists():
    raise FileNotFoundError(f"PDF not found: {pdf_path}")

# Step 1: Read PDF
file_bytes = pdf_path.read_bytes()

# Step 2: Render PDF pages
rendered_pages = render_pdf_pages_to_png(
    file_bytes=file_bytes,
    drawing_id=drawing_id,
    dpi=dpi
)

print(f"Total rendered pages: {len(rendered_pages)}")

output_dir = Path("storage/rendered-pages")

# Step 3: Verify rendered images and metadata
for page in rendered_pages:

    image_path = output_dir / page["relativePath"]

    print({
        "pageNumber": page["pageNumber"],
        "fileName": page["fileName"],
        "relativePath": page["relativePath"],
        "width": page["width"],
        "height": page["height"],
        "dpi": page["dpi"],
        "format": page["format"],
        "sizeBytes": page["sizeBytes"]
    })

    # Verify file exists
    assert image_path.exists(), (
        f"Image not found: {image_path}"
    )

    # Verify file is not empty
    assert image_path.stat().st_size > 0, (
        f"Image is empty: {image_path}"
    )

    # Verify actual file size matches metadata
    actual_size = image_path.stat().st_size

    assert actual_size == page["sizeBytes"], (
        f"File size mismatch: expected {page['sizeBytes']}, "
        f"got {actual_size}"
    )

    # Verify PNG signature
    with image_path.open("rb") as image_file:
        signature = image_file.read(8)

    assert signature == b"\x89PNG\r\n\x1a\n", (
        f"Invalid PNG file: {image_path}"
    )

    # Verify actual PNG pixel dimensions
    actual_pixmap = pymupdf.Pixmap(str(image_path))

    assert actual_pixmap.width == page["width"], (
        f"Width mismatch: expected {page['width']}, "
        f"got {actual_pixmap.width}"
    )

    assert actual_pixmap.height == page["height"], (
        f"Height mismatch: expected {page['height']}, "
        f"got {actual_pixmap.height}"
    )

    print(
        f"Page {page['pageNumber']} validation passed: "
        f"{actual_pixmap.width}x{actual_pixmap.height} pixels"
    )

print("\nAll PDF page rendering metadata tests passed!")
