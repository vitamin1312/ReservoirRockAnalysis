import React from "react";
import { FieldData } from "../../Models/ImageData";

interface FieldTableProps {
  fields: FieldData[];
  onDelete: (field: FieldData) => void;
}

const FieldTable: React.FC<FieldTableProps> = ({ fields, onDelete }) => {
  return (
    <div className="flex flex-col h-full">
      <h3 className="text-xl mb-3">Месторождения</h3>
      <div className="overflow-y-auto h-64"> {/* Ограничиваем высоту таблицы и добавляем скроллинг */}
        <table className="table-auto w-full border-collapse border border-gray-300">
          <thead className="bg-gray-200">
            <tr>
              <th className="px-4 py-2 text-left">Название</th>
              <th className="px-4 py-2 text-left">Описание</th>
              <th className="px-4 py-2 text-left">Действия</th>
            </tr>
          </thead>
          <tbody>
            {fields.length > 0 ? (
              fields.map((field) => (
                <tr key={field.id} className="border-t">
                  <td className="px-4 py-2">{field.name}</td>
                  <td className="px-4 py-2">{field.description}</td>
                  <td className="px-4 py-2">
                    <button
                      onClick={() => onDelete(field)}
                      className="text-red-500 hover:text-red-700"
                    >
                      Удалить
                    </button>
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan={3} className="px-4 py-2 text-center text-gray-500">
                  Нет месторождений
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default FieldTable;
