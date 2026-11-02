from typing import Literal
from pydantic import BaseModel, Field


class PageBox(BaseModel):
    x0: float
    y0: float
    x1: float
    y1: float


class TakeoffItem(BaseModel):
    objectType: str
    objectName: str
    quantity: float
    confidence: float | None = None
    pageNumber: int | None = None


class DocumentMetadata(BaseModel):
    title: str | None = None
    author: str | None = None
    subject: str | None = None
    keywords: str | None = None
    creator: str | None = None
    producer: str | None = None
    creationDate: str | None = None
    modificationDate: str | None = None
    pageCount: int


class TextSpan(BaseModel):
    text: str
    font: str
    fontSize: float
    flags: int
    color: int
    bbox: PageBox



class TextBlock(BaseModel):
    blockNumber: int
    text: str
    bbox: PageBox
    spans: list[TextSpan] = Field(default_factory=list)

class PageStatistics(BaseModel):
    textBlockCount: int = 0
    textSpanCount: int = 0
    characterCount: int = 0
    wordCount: int = 0
    imageBlockCount: int = 0
    hasText: bool = False

class PageAnalysis(BaseModel):
    pageNumber: int
    width: float
    height: float
    widthMm: float
    heightMm: float
    rotation: int
    mediaBox: PageBox
    cropBox: PageBox

    extractedText: str
    textBlocks: list[TextBlock] = Field(default_factory=list)
    statistics: PageStatistics = Field(default_factory=PageStatistics)

class ExtractedImage(BaseModel):
    pageNumber: int
    imageIndex: int
    xref: int

    fileName: str
    relativePath: str

    width: int
    height: int
    extension: str

    colorspace: int | None = None
    bitsPerComponent: int | None = None

    sizeBytes: int
    smask: int = 0

class RenderedPage(BaseModel):
    pageNumber: int

    fileName: str
    relativePath: str

    width: int
    height: int

    dpi: int
    format: str

    sizeBytes: int

class PageClassification(BaseModel):
    pageNumber: int
    classification: Literal[
        "text_based",
        "scanned",
        "mixed",
        "unknown",
    ]
    confidence: float = Field(ge=0.0, le=1.0)
    warnings: list[str] = Field(default_factory=list)


class DocumentClassification(BaseModel):
    classification: Literal[
        "text_based",
        "scanned",
        "mixed",
        "unknown",
    ]
    confidence: float = Field(ge=0.0, le=1.0)
    pageClassifications: list[PageClassification] = Field(
        default_factory=list
    )
    warnings: list[str] = Field(default_factory=list)


class DrawingAnalysisResponse(BaseModel):
    drawingId: str
    processingJobId: str
    status: str
    analysisMode: str
    documentMetadata: DocumentMetadata
    classification: DocumentClassification
    totalPages: int
    items: list[TakeoffItem] = Field(default_factory=list)
    pages: list[PageAnalysis] = Field(default_factory=list)
   
    extractedImages: list[ExtractedImage] = Field(default_factory=list)
    renderedPages: list[RenderedPage] = Field(default_factory=list)
    processingTimeMs: int
