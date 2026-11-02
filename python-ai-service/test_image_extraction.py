
from pathlib import Path

from app.services.pdf_image_extractor import extract_embedded_images
from app.services.image_storage_service import save_extracted_images


pdf_path = Path("sample.pdf")
drawing_id = "drawing-001"

if not pdf_path.exists():
    raise FileNotFoundError(f"PDF not found: {pdf_path}")

# Step 1: Read PDF
file_bytes = pdf_path.read_bytes()

# Step 2: Extract embedded images
images = extract_embedded_images(file_bytes)

print(f"Total embedded images: {len(images)}")

# Step 3: Save extracted images
saved_images = save_extracted_images(
    images=images,
    drawing_id=drawing_id
)

# Step 4: Display saved image metadata
print(f"\nTotal saved images: {len(saved_images)}")

for image in saved_images:
    print({
        "pageNumber": image["pageNumber"],
        "imageIndex": image["imageIndex"],
        "fileName": image["fileName"],
        "relativePath": image["relativePath"],
        "width": image["width"],
        "height": image["height"],
        "extension": image["extension"],
        "sizeBytes": image["sizeBytes"]
    })

    # Verify that the file exists
    saved_file = Path("storage/extracted-images") / image["relativePath"]

    assert saved_file.exists(), f"Image file not found: {saved_file}"
    assert saved_file.stat().st_size > 0, "Image file is empty"

print("\nAll image extraction and storage tests passed!")
