import fitz


def extract_pdf_metadata(file_bytes: bytes) -> dict:
    with fitz.open(
        stream=file_bytes,
        filetype="pdf"
    ) as document:

        metadata = document.metadata or {}

        return {
            "title": metadata.get("title") or None,
            "author": metadata.get("author") or None,
            "subject": metadata.get("subject") or None,
            "keywords": metadata.get("keywords") or None,
            "creator": metadata.get("creator") or None,
            "producer": metadata.get("producer") or None,
            "creationDate": metadata.get("creationDate") or None,
            "modificationDate": metadata.get("modDate") or None,
            "pageCount": len(document)
        }