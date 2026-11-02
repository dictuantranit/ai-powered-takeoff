
from fastapi import APIRouter, UploadFile, File, Form, HTTPException

from app.schemas.analysis import DrawingAnalysisResponse
from app.services.analysis_service import analyze_drawing

router = APIRouter()


@router.post(
    "/analyze",
    response_model=DrawingAnalysisResponse
)
async def analyze(
    file: UploadFile = File(...),
    drawingId: str = Form(...),
    processingJobId: str = Form(...)
):
    if file.content_type != "application/pdf":
        raise HTTPException(
            status_code=400,
            detail="Only PDF files are supported."
        )

    file_bytes = await file.read()

    if not file_bytes:
        raise HTTPException(
            status_code=400,
            detail="Uploaded PDF is empty."
        )

    if len(file_bytes) > 20 * 1024 * 1024:
        raise HTTPException(
            status_code=413,
            detail="PDF file exceeds 20 MB."
        )

    if not file_bytes.startswith(b"%PDF-"):
        raise HTTPException(
            status_code=400,
            detail="Invalid PDF file."
        )

    try:
        return analyze_drawing(
            file_bytes=file_bytes,
            drawing_id=drawingId,
            processing_job_id=processingJobId
        )

    except ValueError as exc:
        raise HTTPException(
            status_code=400,
            detail=str(exc)
        )

    except Exception:
        raise HTTPException(
            status_code=500,
            detail="An unexpected error occurred during PDF analysis."
        )
