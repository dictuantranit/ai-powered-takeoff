
import re
from pathlib import Path


DEFAULT_STORAGE_DIR = Path("storage/extracted-images")


def save_extracted_images(
    images: list[dict],
    drawing_id: str,
    output_dir: Path = DEFAULT_STORAGE_DIR
) -> list[dict]:
    """
    Save extracted PDF images to local storage.

    Returns image metadata without binary image data.
    """

    # Prevent directory traversal through drawing_id.
    if not re.fullmatch(r"[A-Za-z0-9_-]+", drawing_id):
        raise ValueError(
            "drawing_id may only contain letters, numbers, "
            "hyphens and underscores"
        )

    saved_images = []
    drawing_dir = output_dir / drawing_id

    for image in images:
        page_number = image["pageNumber"]
        image_index = image["imageIndex"]
        extension = image["extension"].lower()

        # Validate file extension before creating the file.
        if not re.fullmatch(r"[a-z0-9]+", extension):
            raise ValueError(f"Invalid image extension: {extension}")

        page_dir = drawing_dir / f"page-{page_number:03d}"
        page_dir.mkdir(parents=True, exist_ok=True)

        file_name = f"image-{image_index:03d}.{extension}"
        file_path = page_dir / file_name

        image_bytes = image["imageBytes"]
        file_path.write_bytes(image_bytes)

        relative_path = file_path.relative_to(output_dir)

        saved_images.append({
            "pageNumber": page_number,
            "imageIndex": image_index,
            "xref": image["xref"],
            "fileName": file_name,
            "relativePath": relative_path.as_posix(),
            "width": image["width"],
            "height": image["height"],
            "extension": extension,
            "colorspace": image.get("colorspace"),
            "bitsPerComponent": image.get("bitsPerComponent"),
            "sizeBytes": file_path.stat().st_size,
            "smask": image.get("smask", 0)
        })

    return saved_images
