export interface ImageInfo {
    id: number,
    name: string,
    description: string,
    uploadDate: string,
    creationDate: string,
    fieldId: number | null,
    field: FieldData | null
}

export interface ImageData {
    id: number,
    imageInfoId: number,
    imageInfo: ImageInfo,
}

export interface FieldData {
    id: number | null;
    name: string;
    description: string;
  }