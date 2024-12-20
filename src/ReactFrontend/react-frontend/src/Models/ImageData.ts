export interface ImageInfo {
    id: number,
    name: string,
    description: string,
    uploadDate: string,
    creationDate: string,
    fieldId: number,
    field: any
}

export interface ImageData {
    id: number,
    imageInfoId: number,
    imageInfo: ImageInfo,
}