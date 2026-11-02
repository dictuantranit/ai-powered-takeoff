
import fitz


def extract_text_blocks(page: fitz.Page) -> list[dict]:
    page_dict = page.get_text("dict")
    text_blocks = []
    block_number = 0

    for block in page_dict["blocks"]:
        if block.get("type") != 0:
            continue

        lines = []
        spans_data = []

        for line in block.get("lines", []):
            line_parts = []

            for span in line.get("spans", []):
                span_text = span.get("text", "")

                if not span_text.strip():
                    continue

                line_parts.append(span_text)

                span_bbox = span["bbox"]

                spans_data.append({
                    "text": span_text,
                    "font": span.get("font", ""),
                    "fontSize": round(span.get("size", 0.0), 2),
                    "flags": span.get("flags", 0),
                    "color": span.get("color", 0),
                    "bbox": {
                        "x0": round(span_bbox[0], 2),
                        "y0": round(span_bbox[1], 2),
                        "x1": round(span_bbox[2], 2),
                        "y1": round(span_bbox[3], 2)
                    }
                })

            line_text = "".join(line_parts).strip()

            if line_text:
                lines.append(line_text)

        text = "\n".join(lines).strip()

        if not text:
            continue

        block_number += 1
        bbox = block["bbox"]

        text_blocks.append({
            "blockNumber": block_number,
            "text": text,
            "bbox": {
                "x0": round(bbox[0], 2),
                "y0": round(bbox[1], 2),
                "x1": round(bbox[2], 2),
                "y1": round(bbox[3], 2)
            },
            "spans": spans_data
        })

    return text_blocks
