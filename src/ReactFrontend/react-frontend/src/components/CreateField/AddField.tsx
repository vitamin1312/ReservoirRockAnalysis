import React, { useState } from "react";

interface CreateFieldFormProps {
  onCreate: (name: string, description: string) => void;
}

const CreateFieldForm: React.FC<CreateFieldFormProps> = ({ onCreate }) => {
  const [newFieldName, setNewFieldName] = useState("");
  const [newDescription, setNewDescription] = useState("");

  const handleCreateField = () => {
    if (newFieldName.trim()) {
      onCreate(newFieldName, newDescription);
      setNewFieldName("");
      setNewDescription("");
    }
  };

  return (
    <div className="mb-2">
      <h3 className="text-xl mb-2">Создание месторождения</h3>
      <input
        type="text"
        value={newFieldName}
        onChange={(e) => setNewFieldName(e.target.value)}
        placeholder="Название месторождения"
        className="px-4 py-2 border border-gray-300 rounded-md w-full mb-3"
      />

        <div>
        <label className="block text-gray-700 text-sm font-medium mb-1">Описание:</label>
        <textarea
            value={newDescription}
            onChange={(e) => setNewDescription(e.target.value)}
            className="w-full h-16 border border-gray-300 rounded-md p-2 focus:ring-blue-500 focus:border-blue-500 resize-none"
        />
        </div>
      <button
        onClick={handleCreateField}
        className="bg-blue-500 text-white py-2 px-6 rounded-md hover:bg-blue-700"
      >
        Создать
      </button>
    </div>
  );
};

export default CreateFieldForm;