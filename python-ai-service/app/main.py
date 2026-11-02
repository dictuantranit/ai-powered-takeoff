
from fastapi import FastAPI

from app.api.routes.analysis import router as analysis_router

app = FastAPI(
    title="DrawingAnalyzer AI Inference Service",
    description="Local AI service for architectural drawing analysis",
    version="1.0.0"
)


@app.get("/health")
def health():
    return {
        "status": "healthy",
        "service": "drawing-analyzer-ai"
    }


app.include_router(
    analysis_router,
    prefix="/api/v1",
    tags=["Drawing Analysis"]
)
