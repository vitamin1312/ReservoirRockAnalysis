import axios from "axios";
import { ImageData } from "../Models/ImageData";

const axiosInstance = axios.create({
  baseURL: "/api/",
});

axiosInstance.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("jwtToken");

    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export const getImagesFromField = async (fieldId: number): Promise<Array<ImageData>> => {
    try {
        const response = await axiosInstance.get(`CoreSampleImages/getfromfield/${fieldId}`);
        return response.data;
      } catch (error) {
        console.error('Error fetching images:', error);
        throw error;
      }
}

export const getImageFile = async (imageId: number): Promise<Blob> => {
  try {
    const response = await axiosInstance.get(`CoreSampleImages/getimagefile/${imageId}`, {
      responseType: 'blob',
    });
    return response.data;
  } catch (error) {
    console.error('Error fetching images:', error);
    throw error;
  }
};

export const getAllImages = async (): Promise<Array<ImageData>> => {
  try {
      const response = await axiosInstance.get(`CoreSampleImages/get`);
      return response.data;
    } catch (error) {
      console.error('Error fetching images:', error);
      throw error;
    }
}

export const authUser = async (login: string, password: string): Promise<string> => {

  const response = await axiosInstance.post("Account/login?", {
    login,
    password,
  });

  return response.data.token;
}