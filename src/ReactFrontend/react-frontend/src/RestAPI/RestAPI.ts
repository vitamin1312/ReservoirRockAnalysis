import axios from "axios";
import { FieldData, ImageData } from "../Models/ImageData";
import { FilterParams } from "../Models/Filter";
import CryptoJS from "crypto-js";

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

export const getImagesWithMask = async (): Promise<Array<ImageData>> => {
  try {
      const response = await axiosInstance.get(`CoreSampleImages/getwithmask`);
      return response.data;
    } catch (error) {
      console.error('Error fetching images:', error);
      throw error;
    }
}

export const getImagesWithoutMask = async (): Promise<Array<ImageData>> => {
  try {
      const response = await axiosInstance.get(`CoreSampleImages/getwithoutmask`);
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

  const hashedPassword = CryptoJS.MD5(password).toString();

  const response = await axiosInstance.post("Account/login?", {
    login,
    password: hashedPassword,
  });

  return response.data.token;
}

export const getAllFields = async (): Promise<Array<FieldData>> => {
  try {
      const response = await axiosInstance.get(`fields/get`);
      return response.data;
    } catch (error) {
      console.error('Error fetching images:', error);
      throw error;
    }
}

export const getImagesByFilter = async (params: FilterParams): Promise<Array<ImageData>> => {
  try {
    let images: Array<ImageData> = [];

    if (params.haveMask === undefined && (params.sortField === undefined || params.sortField === -1)) {
      images = await getAllImages();
    } else if (params.haveMask !== undefined && (params.sortField === undefined || params.sortField === -1)) {
      const func = params.haveMask ? getImagesWithMask : getImagesWithoutMask;
      images = await func();
    } else if (params.haveMask === undefined && (params.sortField !== undefined && params.sortField !== -1)) {
      images = await getImagesFromField(params.sortField);
    } else if (params.haveMask !== undefined && params.sortField !== undefined) {
      const imagesFromField = await getImagesFromField(params.sortField);
      const func = params.haveMask ? getImagesWithMask : getImagesWithoutMask;
      const maskImages = await func();
      const maskImageIds = new Set(maskImages.map((image) => image.id));
      images = imagesFromField.filter((image) => maskImageIds.has(image.id));
    }

    if (params.searchQuery !== undefined) {
      const query: string = params.searchQuery;
      images = images.filter(image => {
        const name = image.imageInfo?.name || "";
        const description = image.imageInfo?.description || "";
        return name.includes(query) || description.includes(query);
      });
    }

    if (params.ascendingOrder) {
      return [...images].sort((a, b) => a.id - b.id);
    } else {
      return [...images].sort((a, b) => b.id - a.id);
    }

  } catch (error) {
    console.error('Error fetching images:', error);
    throw error;
  }
};
