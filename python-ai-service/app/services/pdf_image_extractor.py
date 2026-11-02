
import pymupdf


def extract_embedded_images(file_bytes: bytes) -> list[dict]:
    """
    Extract embedded images from a PDF file.

    Returns image metadata and original image bytes.
    Image bytes are kept internally and are not intended
    to be returned directly through an API response.
    """
    extracted_images = []

    with pymupdf.open(stream=file_bytes, filetype="pdf") as document:
        for page_index, page in enumerate(document):
            images = page.get_images(full=True)

            for image_index, image_info in enumerate(images, start=1):
                xref = image_info[0]
                smask = image_info[1]

                if xref <= 0:
                    continue

                image_data = document.extract_image(xref)

                if not image_data:
                    continue

                image_bytes = image_data["image"]

                extracted_images.append({
                    "pageNumber": page_index + 1,
                    "imageIndex": image_index,
                    "xref": xref,
                    "width": image_data["width"],
                    "height": image_data["height"],
                    "extension": image_data["ext"],
                    "colorspace": image_data.get("colorspace"),
                    "bitsPerComponent": image_data.get("bpc"),
                    "sizeBytes": len(image_bytes),
                    "smask": smask,
                    "imageBytes": image_bytes
                })

    return extracted_images
