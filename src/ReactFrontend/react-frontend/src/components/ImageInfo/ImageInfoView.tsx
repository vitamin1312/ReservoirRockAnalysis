import React, { useState, useEffect } from 'react';
import { ImageData, FieldData } from '../../Models/ImageData';
import ImageConvert from './ImageConvert';
import { getImageUrl, getImageWithMaskUrl, getAllFields, getMaskUrl, getMaskImageUrl } from '../../RestAPI/RestAPI';
import ImageComponent from '../ImageDisplay/ImageComponent';
import RadioGroup from '../UI/RadioGroup';
import Dropdown from '../UI/Dropdown';

interface ImageInfoProps {
  image: ImageData | null;
  fetchImages: () => void;
}

const ImageInfoView: React.FC<ImageInfoProps> = ({ image, fetchImages }) => {
  const [fieldsData, setFieldsData] = useState<Array<FieldData>>([]);
  const [selectedFieldId, setSelectedFieldId] = useState<number>(image?.imageInfo.fieldId || 0);
  const [currentImage, setCurrentImage] = useState<ImageData | null>(image);
  const [selectedFunction, setSelectedFunction] = useState<string>('imageFunc');
  const [pixelLengthValue, setpixelLengthValue] = useState("");

    const handlepixelLengthValueChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = e.target.value;

    // Разрешаем только числа и одну точку
    if (/^\d*\.?\d*$/.test(newValue)) {
      setpixelLengthValue(newValue);
      if (image != null) {
         image.imageInfo.pixelLengthRatio = Number(newValue);
        }
      }
    };

  const urlFunctions: Record<string, (id: number) => Promise<string>> = {
    imageFunc: getImageUrl,
    maskFunc: getMaskUrl,
    maskImageFunc: getMaskImageUrl,
    imagewithmaskFunc: getImageWithMaskUrl,
  };

  const fetchFields = async () => {
    try {
      const data = await getAllFields();
      setFieldsData(data);
    } catch (error) {
      console.error('Error fetching fields:', error);
    }
  };

  const fetchImage = async () => {
    try {
      if (currentImage) {
      
        const url: string = await urlFunctions[selectedFunction](currentImage.id);
        setCurrentImage(prev => ({
          ...prev!,
          imageInfo: {
            ...prev!.imageInfo,
            url
          }
        }));
      }
    } catch (error) {
      console.error('Error fetching image URL:', error);
    }
  };

  useEffect(() => {

    fetchFields();
  }, []);

  useEffect(() => {
    setCurrentImage(image);
    if (image) {
      setSelectedFieldId(image.imageInfo.fieldId?? 0);
      setpixelLengthValue(String(image.imageInfo.pixelLengthRatio));
    }
  }, [image]);

  useEffect(() => {
    if (currentImage) {
      fetchImage();
    }
  }, [selectedFunction, currentImage?.id]);

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
      fetchImage();
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
    setCurrentImage(null);
  };

  return (
    <>
      {currentImage ? (
        <div>
          <div className="flex flex-col bg-gray-100 p-6 shadow-md overflow-auto space-y-6">
            <div className="w-full flex justify-center">
              <div className="w-3/4">
                {/* Передаем функцию для загрузки изображения */}
                <ImageComponent
                  imageId={currentImage.id}
                  getImage={urlFunctions[selectedFunction]}
                />
              </div>
            </div>

            <h3 className="text-xl font-semibold text-gray-800 border-b pb-2">Выбор типа изображения</h3>
            <RadioGroup
              options={[
                { value: 'imageFunc', label: 'Изображение' },
                { value: 'maskFunc', label: 'Маска'},
                { value: 'maskImageFunc', label: 'Изображение маски'},
                { value: 'imagewithmaskFunc', label: 'Изображение с маской'}
              ]}
              name="urlFunction"
              selectedValue={selectedFunction}
              onChange={setSelectedFunction}
            />

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

                <div>
                <label className="block text-gray-700 text-sm font-medium mb-1">Физическая длина пикселя:</label>
                    <input
                      type="text"
                      value={pixelLengthValue}
                      onChange={handlepixelLengthValueChange}
                      placeholder="Введите число"
                      className="w-full border border-gray-300 rounded-md p-2 focus:ring-blue-500 focus:border-blue-500"
                    />
              </div>

              <div className="relative">
                <Dropdown
                  label="Месторождение:"
                  placeholder="Выберите месторождение"
                  options={[{ id: -1, name: 'Нет месторождения', description: 'Нет месторождения' }, ...fieldsData]}
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
                <ImageConvert image={currentImage} onImageDeleted={handleImageDeleted} fetchImages={fetchImages}/>
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
