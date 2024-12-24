export interface ImageInfo {
    id: number,
    name: string,
    description: string,
    uploadDate: string,
    creationDate: string,
    fieldId: number,
    field: FieldData
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