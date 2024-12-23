import axios from "axios";
import { ImageData } from "../Models/ImageData";

export const getImagesFromField = async (fieldId: number): Promise<Array<ImageData>> => {
    try {
        const response = await axios.get(`/api/CoreSampleImages/getfromfield/${fieldId}`);
        return response.data;
      } catch (error) {
        console.error('Error fetching images:', error);
        throw error;
      }
}

export const getImageFile = async (imageId: number): Promise<Blob> => {
  try {
    const response = await axios.get(`api/CoreSampleImages/getimagefile/${imageId}`, {
      responseType: 'blob',
    });
    return response.data;
  } catch (error) {
    console.error('Error fetching images:', error);
    throw error;
  }
};