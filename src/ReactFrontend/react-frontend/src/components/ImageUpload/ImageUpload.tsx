import React, { useState, useEffect } from "react";
import Dropdown from "../UI/Dropdown";
import { FieldData } from "../../Models/ImageData";
import { getAllFields } from "../../RestAPI/RestAPI";
import Select from 'react-select';

interface ImageUploadFormProps {
  onUpload: (file: File, imageType: number, description: string, fieldId: number, pixelLengthRatio: string) => void;
  fields: FieldData[];
  refreshFields: () => void;
}


const options = [
  { value: 0, label: 'Изображение' },
  { value: 1, label: 'Маска' },
  { value: 2, label: 'Изображение маски' },
];

const ImageUploadForm: React.FC<ImageUploadFormProps> = ({ onUpload, fields }) => {
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [newDescription, setNewDescription] = useState("");
  const [uploadError, setUploadError] = useState<string | null>(null);
  const [selectedFieldId, setSelectedFieldId] = useState<number>(-1);
  const [_, setFieldsData] = useState<Array<FieldData>>([]);

  const [pixelLengthValue, setpixelLengthValue] = useState(localStorage.getItem("pixelLengthValue") || "");

  const [isUploading, setIsUploading] = useState(false);

  const handlepixelLengthValueChange = (e: React.ChangeEvent<HTMLInputElement>) => {
  let newValue = e.target.value;

  if (/^\d*[.,]?\d*$/.test(newValue)) {
    newValue = newValue.replace('.', ',');
    setpixelLengthValue(newValue);
    localStorage.setItem("pixelLengthValue", newValue);
    }
  };


  const [selectedOption, setSelectedOption] = useState(options[Number(localStorage.getItem("imageType") || 0)]);


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

  const handleUpload = async () => {
  if (!imageFile) {
    setUploadError("Пожалуйста, выберите файл для загрузки.");
    return;
  }

  if (pixelLengthValue == '') {
    setUploadError("Пожалуйста, введите физическую длину пикселя");
    return;
  }

  setUploadError(null);
  setIsUploading(true);
  try {
    await onUpload(
      imageFile,
      selectedOption.value,
      newDescription,
      selectedFieldId,
      pixelLengthValue.replace(',', '.')
    );
  } catch (error) {
    setUploadError(`${error}`);
    console.error(error);
  } finally {
    setIsUploading(false);
  }
};

   return (
    <div className="container mx-auto p-2 space-y-3 flex flex-col overflow-y-auto">
      <div className="flex flex-col md:flex-row md:items-center md:gap-4 mt-auto">
        <input
          type="file"
          onChange={handleFileChange}
          className="px-4 py-2 border border-gray-300 rounded-md mb-1 md:mb-0 md:w-1/3"
        />
        <div className="px-4 py-2 rounded-md mb-1 md:mb-0 md:w-1/3 h-full">
         <Select
            options={options}
            value={selectedOption}
            onChange={(option) => {
              setSelectedOption(option!);
              if (option?.value) {
                localStorage.setItem("imageType", String(option.value));
              }
            }}
          />
          
        </div>
         <div className="px-4 py-2 rounded-md mb-1 md:mb-0 md:w-1/3 h-full">
         <input
              type="text"
              value={pixelLengthValue}
              onChange={handlepixelLengthValueChange}
              placeholder="Физическая длина пикселя (введите число)"
              className="w-full border border-gray-300 rounded-md p-1.5 focus:ring-blue-500 focus:border-blue-500"
            />
            </div>
       
      </div>
        <Dropdown
          label="Месторождение:"
          placeholder="Выберите месторождение"
          options={[{ id: -1, name: "Нет месторождения", description: "Нет месторождения" }, ...fields]}
          onSelect={handleFieldSelect}
          selectedId={selectedFieldId}
        />
      <h3 className="text-xl mb-3">Описание изображения</h3>
      <textarea
        value={newDescription}
        onChange={(e) => setNewDescription(e.target.value)}
        className="w-full border border-gray-300 rounded-md p-2 focus:ring-blue-500 focus:border-blue-500 resize-none flex-grow"
      />
      {uploadError && <div className="text-red-500">{uploadError}</div>}
      <button
        onClick={handleUpload}
        disabled={isUploading}
        className="bg-green-300 text-black font-semibold py-2 px-6 rounded-md shadow-md transition-all hover:bg-green-400 focus:outline-none focus:ring-4 focus:ring-blue-300 active:scale-95"
      >
        {isUploading ? 'Загрузка...' : 'Загрузить'}
      </button>
    </div>
  );
};

export default ImageUploadForm;