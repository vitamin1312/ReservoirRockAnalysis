import React from "react";
import { ImageData } from "../../Models/ImageData";
import { deleteImage, generateMask, putImage } from "../../RestAPI/RestAPI";

interface ImageConvertProps {
  image: ImageData | null;
  onImageDeleted: () => void; // Callback для уведомления об удалении
}

const ImageConvert: React.FC<ImageConvertProps> = ({ image, onImageDeleted }) => {
    if (!image) {
      return <div>Выберите изображение для обработки.</div>;
    }

    const handleGenerateMask = async () => {
        try {
            await generateMask(image.id);
            alert("Маска успешно сгенерирована");
        } catch (err) {
            alert("Не удалось сгенерировать маску: " + String(err))
        }
        
      };

    const handleSaveChanges = async () => {
        try {
          await putImage(image.id, image);
          onImageDeleted();
        } catch (error) {
          console.error("Ошибка при удалении изображения:", error);
          alert("Не удалось сохранить изображение.");
        }
      };
  
    const handleDeleteImage = async () => {
      try {
        await deleteImage(image.id);
        onImageDeleted();
      } catch (error) {
        console.error("Ошибка при удалении изображения:", error);
        alert("Не удалось удалить изображение.");
      }
    };
  
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
          className="bg-green-500 text-white px-4 py-2 rounded-md hover:bg-green-600 m-0.5"
          onClick={handleGenerateMask}
        >
          Сгенерировать маску
        </button>
      </div>
    );
};
  
export default ImageConvert;
