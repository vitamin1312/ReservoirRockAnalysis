import React, { useState, useEffect } from "react";
import Dropdown from "../UI/Dropdown";
import { FieldData } from "../../Models/ImageData";
import { getAllFields } from "../../RestAPI/RestAPI";

interface ImageUploadFormProps {
  onUpload: (file: File, description: string, fieldId: number) => void;
}

const ImageUploadForm: React.FC<ImageUploadFormProps> = ({ onUpload }) => {
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [newDescription, setNewDescription] = useState("");
  const [uploadError, setUploadError] = useState<string | null>(null);
  const [selectedFieldId, setSelectedFieldId] = useState<number>(-1);
  const [fieldsData, setFieldsData] = useState<Array<FieldData>>([]);

  const handleFieldSelect = (id: number) => {
    setSelectedFieldId(id);
    };
const fetchFields = async () => {
    try {
        const data = await getAllFields();
        setFieldsData(data);
    } catch (error) {
        console.error('Error fetching fields:', error);
    }
    };

  useEffect(() => {
    fetchFields();
  }, []);


  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files ? e.target.files[0] : null;
    setImageFile(file);
    fetchFields();
  };

  const handleUpload = () => {
    if (!imageFile) {
      setUploadError("Пожалуйста, выберите файл для загрузки.");
      return;
    }
    setUploadError(null);
    onUpload(imageFile, newDescription, selectedFieldId);
  };

  return (
    <div className="flex flex-col h-full">
      <div className="overflow-y-auto h-44 md-4">
        <input
            type="file"
            onChange={handleFileChange}
            className="px-4 py-2 border border-gray-300 rounded-md mb-3"
        />
        <Dropdown
            label="Месторождение:"
            placeholder="Выберите месторождение"
            options={[{ id: -1, name: 'Нет месторождения', description: 'Нет месторождения' }, ...fieldsData]}
            onSelect={handleFieldSelect}
            selectedId={selectedFieldId}
        />
        <h3 className="text-xl mb-3">Описание изображения</h3>
        <textarea
            value={newDescription}
            onChange={(e) => setNewDescription(e.target.value)}
            className="w-full h-16 border border-gray-300 rounded-md p-2 focus:ring-blue-500 focus:border-blue-500 resize-none"
        />
        {uploadError && <div className="text-red-500">{uploadError}</div>}
        <button
            onClick={handleUpload}
            className="bg-green-300 text-black font-semibold py-2 px-6 rounded-md shadow-md transition-all hover:bg-green-400 focus:outline-none focus:ring-4 focus:ring-blue-300 active:scale-95"
        >
            Загрузить
        </button>
        </div>
    </div>
  );
};

export default ImageUploadForm;