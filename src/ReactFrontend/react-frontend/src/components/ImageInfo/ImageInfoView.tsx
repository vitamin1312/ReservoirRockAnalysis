import React, { useState, useEffect } from 'react';
import { ImageData } from '../../Models/ImageData';
import ImageConvert from './ImageConvert';
import { getImageUrl, getAllFields } from '../../RestAPI/RestAPI';
import ImageComponent from '../ImageDisplay/ImageComponent';
import { FieldData } from '../../Models/ImageData';
import Dropdown from '../UI/Dropdown';

interface ImageInfoProps {
  image: ImageData | null;
}

const ImageInfoView: React.FC<ImageInfoProps> = ({ image }) => {
  const [fieldsData, setFieldsData] = useState<Array<FieldData>>([]);
  const [selectedFieldId, setSelectedFieldId] = useState<number>(image?.imageInfo.fieldId || 0);
  const [currentImage, setCurrentImage] = useState<ImageData | null>(image);

  useEffect(() => {
    const fetchFields = async () => {
      try {
        const data = await getAllFields();
        setFieldsData(data);
      } catch (error) {
        console.error('Error fetching fields:', error);
      }
    };

    fetchFields();
  }, []);

  useEffect(() => {
    setCurrentImage(image); // Обновляем изображение при изменении
    if (image) {
      setSelectedFieldId(image.imageInfo.fieldId); // Обновляем месторождение
    }
  }, [image]);

  const handleFieldSelect = (id: number) => {
    setSelectedFieldId(id);
    if (currentImage) {
      setCurrentImage({
        ...currentImage,
        imageInfo: { ...currentImage.imageInfo, fieldId: id },
      });
    }
  };

  const handleNameChange = (name: string) => {
    if (currentImage) {
      setCurrentImage({
        ...currentImage,
        imageInfo: { ...currentImage.imageInfo, name },
      });
    }
  };

  const handleDescriptionChange = (description: string) => {
    if (currentImage) {
      setCurrentImage({
        ...currentImage,
        imageInfo: { ...currentImage.imageInfo, description },
      });
    }
  };

  const handleImageDeleted = () => {
    setCurrentImage(null); // Обновляем состояние после удаления
  };

  return (
    <>
      {currentImage ? (
        <div>
          <div className="flex flex-col bg-gray-100 p-6 shadow-md overflow-auto space-y-6">
            <div className="w-full flex justify-center">
              <div className="w-3/4">
                <ImageComponent imageId={currentImage.id} getImage={getImageUrl} />
              </div>
            </div>

            <h3 className="text-xl font-semibold text-gray-800 border-b pb-2">Информация</h3>

            <div className="space-y-4">
              <div>
                <label className="block text-gray-700 text-sm font-medium mb-1">Название:</label>
                <input
                  type="text"
                  value={currentImage.imageInfo.name}
                  onChange={(e) => handleNameChange(e.target.value)}
                  className="w-full border border-gray-300 rounded-md p-2 focus:ring-blue-500 focus:border-blue-500"
                />
              </div>

              <div className="relative">
                <Dropdown
                  label="Месторождение:"
                  placeholder="Выберите месторождение"
                  options={[{ id: null, name: 'Нет месторождения', description: 'Нет месторождения' }, ...fieldsData]}
                  onSelect={handleFieldSelect}
                  selectedId={selectedFieldId}
                />
              </div>

              <div>
                <label className="block text-gray-700 text-sm font-medium mb-1">Описание:</label>
                <textarea
                  value={currentImage.imageInfo.description}
                  onChange={(e) => handleDescriptionChange(e.target.value)}
                  className="w-full h-24 border border-gray-300 rounded-md p-2 focus:ring-blue-500 focus:border-blue-500 resize-none"
                />
              </div>

              <div>
                <label className="block text-gray-700 text-sm font-medium mb-1">Дата загрузки:</label>
                <input
                  type="text"
                  value={new Date(currentImage.imageInfo.uploadDate).toLocaleString() || 'Неизвестно'}
                  disabled
                  className="w-full border border-gray-300 rounded-md p-2 bg-gray-200 text-gray-500 cursor-not-allowed"
                />
              </div>
              <div>
                <ImageConvert image={currentImage} onImageDeleted={handleImageDeleted} />
              </div>
            </div>
          </div>
        </div>
      ) : (
        <div className="m-5">Изображение было изменено, удалено или не выбрано.</div>
      )}
    </>
  );
};

export default ImageInfoView;
