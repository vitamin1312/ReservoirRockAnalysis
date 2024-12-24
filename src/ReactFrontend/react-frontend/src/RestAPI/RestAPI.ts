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

export const getImageWithMaskFile = async (imageId: number): Promise<Blob> => {
  try {
    const response = await axiosInstance.get(`CoreSampleImages/getimagewithmaskfile/${imageId}`, {
      responseType: 'blob',
    });
    if (response.status === 200) {
      return response.data;
    } else {
      throw new Error(`Error fetching image with mask: ${response.statusText}`);
    }
  } catch (error: any) {
    if (error.response && error.response.status === 404) {
      throw new Error('Маска для этого изображения не сгенерирована.');
    } else {
      console.error('Error fetching image with mask:', error);
      throw new Error('Ошибка при загрузке изображения с маской.');
    }
  }
};


export const getMaskFile = async (imageId: number): Promise<Blob> => {
  try {
    const response = await axiosInstance.get(`CoreSampleImages/getmaskfile/${imageId}`, {
      responseType: 'blob',
    });
    if (response.status === 200) {
      return response.data;
    } else {
      throw new Error(`Error fetching image with mask: ${response.statusText}`);
    }
  } catch (error: any) {
    if (error.response && error.response.status === 404) {
      throw new Error('Маска для этого изображения не сгенерирована.');
    } else {
      console.error('Error fetching image with mask:', error);
      throw new Error('Ошибка при загрузке изображения с маской.');
    }
  }
};

export const getMaskImageFile = async (imageId: number): Promise<Blob> => {
  try {
    const response = await axiosInstance.get(`CoreSampleImages/getmaskimagefile/${imageId}`, {
      responseType: 'blob',
    });
    if (response.status === 200) {
      return response.data;
    } else {
      throw new Error(`Error fetching image with mask: ${response.statusText}`);
    }
  } catch (error: any) {
    if (error.response && error.response.status === 404) {
      throw new Error('Маска для этого изображения не сгенерирована.');
    } else {
      console.error('Error fetching image with mask:', error);
      throw new Error('Ошибка при загрузке изображения с маской.');
    }
  }
};


export const getImageUrl = async (imageId: number): Promise<string> => {
  try {
    const file = await getImageFile(imageId);
    return window.URL.createObjectURL(new Blob([file]));
  } catch (error) {
    console.error('Error fetching images:', error);
    throw error;
  }
};


export const getImageWithMaskUrl = async (imageId: number): Promise<string> => {
  try {
    const file = await getImageWithMaskFile(imageId);
    return window.URL.createObjectURL(new Blob([file]));
  } catch (error) {
    console.error('Error fetching images:', error);
    throw error;
  }
};

export const getMaskUrl = async (imageId: number): Promise<string> => {
  try {
    const file = await getMaskFile(imageId);
    return window.URL.createObjectURL(new Blob([file]));
  } catch (error) {
    console.error('Error fetching images:', error);
    throw error;
  }
};

export const getMaskImageUrl = async (imageId: number): Promise<string> => {
  try {
    const file = await getMaskImageFile(imageId);
    return window.URL.createObjectURL(new Blob([file]));
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
      const query: string = params.searchQuery.toLocaleLowerCase();
      images = images.filter(image => {
        const name = image.imageInfo?.name.toLocaleLowerCase() || "";
        const description = image.imageInfo?.description.toLocaleLowerCase() || "";
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

export const deleteImage = async (imageId: number): Promise<void> => {
  try {
      const response = await axiosInstance.delete(`CoreSampleImages/deleteitem/${imageId}`);
      return response.data;
    } catch (error) {
      console.error('Error fetching images:', error);
      throw error;
    }
}

export const putImage = async (imageId: number, imageData: ImageData): Promise<void> => {
  try {
    const response = await axiosInstance.put(`CoreSampleImages/putitem/${imageId}`,       {
      id: imageData.imageInfo.id,
      name: imageData.imageInfo.name,
      description: imageData.imageInfo.description,
      uploadDate: imageData.imageInfo.uploadDate,
      creationDate: imageData.imageInfo.creationDate,
      fieldId: imageData.imageInfo.fieldId,
      field: imageData.imageInfo.field,
    },
    {
      headers: {
        'Content-Type': 'application/json',
      },
    }
  );
    return response.data;
  } catch (error) {
    console.error('Error updating image:', error);
    throw error;
  }
};


export const generateMask = async (fieldId: number): Promise<void> => {
  try {
      const response = await axiosInstance.get(`CoreSampleImages/predict/${fieldId}`);
      return response.data;
    } catch (error) {
      console.error('Error fetching images:', error);
      throw error;
    }
}