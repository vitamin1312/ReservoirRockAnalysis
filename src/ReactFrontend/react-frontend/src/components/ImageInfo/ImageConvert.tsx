import React, { useState } from "react";
import { ImageData } from "../../Models/ImageData";
import { deleteImage, generateMask, putImage, downloadPorosityExcel } from "../../RestAPI/RestAPI";

interface ImageConvertProps {
  image: ImageData | null;
  onImageDeleted: () => void;
  fetchImages: () => void;
}

const ImageConvert: React.FC<ImageConvertProps> = ({ image, onImageDeleted, fetchImages }) => {

  const [isGeneratingMask, setIsGeneratingMask] = useState(false);
  const [isGeneratingExcel, setIsGeneratingExcel] = useState(false);

    if (!image) {
      return <div>Выберите изображение для обработки.</div>;
    }

    const handleGenerateMask = async () => {
        try {
            setIsGeneratingMask(true);
            await generateMask(image.id);
            alert("Маска успешно сгенерирована");
            fetchImages();
        } catch (err) {
            alert("Не удалось сгенерировать маску: " + String(err))
        } finally {
          setIsGeneratingMask(false);
        }
      };

    const handleSaveChanges = async () => {
        try {
          await putImage(image.id, image);
          onImageDeleted();
          fetchImages();
        } catch (error) {
          console.error("Ошибка при сохранении изображения:", error);
          alert("Не удалось сохранить изображение.");
        }
      };
  
    const handleDeleteImage = async () => {
      try {
        await deleteImage(image.id);
        onImageDeleted();
        fetchImages();
      } catch (error) {
        console.error("Ошибка при удалении изображения:", error);
        alert("Не удалось удалить изображение.");
      }
    };

    const handleGetPososity = async () => {
      try {
        await downloadPorosityExcel(image.id, image.imageInfo.name, image.imageInfo.pixelLengthRatio)
        setIsGeneratingExcel(true)
      } catch (error) {
        console.error("Ошибка при обработке изображения изображения:", error);
        alert("Не удалось получить информацию о пористости изображения.");
      } finally {
          setIsGeneratingExcel(false);
        }
    }
  
    return (
      <div className="flex flex-row justify-center items-center m-1 mb-16">
        <button
          className="bg-blue-500 text-white px-4 py-2 rounded-md hover:bg-blue-600 m-0.5"
          onClick={handleSaveChanges}
        >
          Сохранить изменения
        </button>
        <button
          className="bg-red-500 text-white px-4 py-2 rounded-md hover:bg-red-600 m-0.5"
          onClick={handleDeleteImage}
        >
          Удалить изображение
        </button>

        <button
          disabled={isGeneratingMask}
          className="bg-green-500 text-white px-4 py-2 rounded-md hover:bg-green-600 m-0.5"
          onClick={handleGenerateMask}
        >
          {isGeneratingMask ? "Генерация..." : "Сгенерировать маску"}
        </button>

        <button
          disabled={isGeneratingExcel}
          className="bg-indigo-500 text-white px-4 py-2 rounded-md hover:bg-indigo-600 m-0.5"
          onClick={handleGetPososity}
        >
          {isGeneratingExcel ? "Генерация..." : "Параметры пористости"}
        </button>

      </div>
    );
};
  
export default ImageConvert;
