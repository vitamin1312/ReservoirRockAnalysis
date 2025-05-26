import React, { useState, useEffect } from "react";
import { ImageData, FieldData } from "../../Models/ImageData";
import FieldTable from "../CreateField/FieldTable";
import CreateFieldForm from "../CreateField/AddField";
import ImageUploadForm from "../ImageUpload/ImageUpload";
import { getAllFields, uploadImage } from "../../RestAPI/RestAPI";
import { deleteField, createField } from "../../RestAPI/RestAPI";

const ImageUploadPage: React.FC = () => {
  const [fieldsData, setFieldsData] = useState<FieldData[]>([]);
  const [imageData, _] = useState<ImageData | null>(null);

  const handleFieldDelete = async (field: FieldData) => {
    await deleteField(field.id ?? -1);
    fetchFields();
  };

  const handleCreateField = async (name: string, description: string) => {
    await createField(name, description);
    fetchFields();
  };

  const handleImageUpload = async (file: File, imageType: number, description: string, fieldId: number) => {
    await uploadImage(file, imageType, description, fieldId);
    fetchFields();
  };

  const fetchFields = async () => {
    try {
      const data = await getAllFields();
      setFieldsData(data);
    } catch (error) {
      console.error("Error fetching fields:", error);
    }
  };

  useEffect(() => {
    fetchFields();
  }, []);

  return (
    <div className="container mx-auto p-2 space-y-3 flex flex-col h-full">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 flex-grow">
        {/* Таблица месторождений */}
        <div className="bg-white shadow-md rounded-lg p-4 overflow-y-auto border">
          <h3 className="text-xl font-semibold text-gray-700 mb-4">Список месторождений</h3>
          <FieldTable fields={fieldsData} onDelete={handleFieldDelete} />
        </div>

        {/* Форма создания месторождения */}
        <div className="bg-white shadow-md rounded-lg p-4 overflow-y-auto border">
          <h3 className="text-xl font-semibold text-gray-700 mb-4">Создать месторождение</h3>
          <CreateFieldForm onCreate={handleCreateField} />
        </div>
      </div>

      {/* Форма загрузки изображения */}
      <div className="bg-white shadow-md rounded-lg p-4 border overflow-y-auto flex-grow">
        <h3 className="text-xl font-semibold text-gray-700 mb-4">Загрузить изображение</h3>
        <ImageUploadForm onUpload={handleImageUpload} />
      </div>

      {/* Загруженные данные об изображении */}
      {imageData && (
        <div className="mt-8 bg-white shadow-md rounded-lg p-4 border">
          <h3 className="text-xl font-semibold text-gray-700 mb-4">Информация о загруженном изображении</h3>
          <p className="text-gray-600"><strong>Название:</strong> {imageData.imageInfo.name}</p>
          <p className="text-gray-600"><strong>Описание:</strong> {imageData.imageInfo.description}</p>
          <p className="text-gray-600"><strong>Дата загрузки:</strong> {new Date().toISOString()}</p>
        </div>
      )}
    </div>
  );
};

export default ImageUploadPage;
