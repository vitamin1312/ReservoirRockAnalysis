import React, { useState } from "react";
import { FieldData } from "../../Models/ImageData";

type DropdownProps = {
  options: FieldData[];
  placeholder: string;
  label: string;
  onSelect: (id: number) => void;
  selectedId: number; // Прямо получаем selectedId от родителя
};

export default function Dropdown({
  options,
  label,
  placeholder,
  onSelect,
  selectedId,
}: DropdownProps) {
  const [isOpen, setIsOpen] = useState<boolean>(false);
  const [searchParam, setSearchParam] = useState<string>("");

  const filteredOptions = options.filter((option) =>
    option.name.toLowerCase().includes(searchParam.toLowerCase())
  );

  const toggleDropdown = () => setIsOpen(!isOpen);

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchParam(e.target.value);
    setIsOpen(true);
  };

  const handleOptionClick = (option: FieldData) => {
    onSelect(option.id ?? 0);
    setIsOpen(false);
  };

  // Найти выбранное значение
  const selectedOptionName = options.find((option) => option.id === selectedId)?.name || "";

  return (
    <div>
      <label className="block text-sm font-medium text-gray-900">{label}</label>

      <div className="relative">
        <input
          type="text"
          value={isOpen ? searchParam : selectedOptionName}
          onClick={toggleDropdown}
          onChange={handleSearchChange}
          placeholder={placeholder}
          className="mt-1 px-4 py-2 w-full bg-white border border-primary rounded-lg shadow-sm focus:outline-none"
        />

        {isOpen && (
          <div className="absolute z-20 mt-1 bg-white border rounded-lg max-h-60 overflow-y-auto w-full">
            {filteredOptions.length > 0 ? (
              <ul>
                {filteredOptions.map((option: FieldData) => (
                  <li
                    key={option.id}
                    onClick={() => handleOptionClick(option)}
                    className="px-4 py-2 hover:bg-blue-400 hover:text-white cursor-pointer"
                  >
                    {option.name}
                  </li>
                ))}
              </ul>
            ) : (
              <p className="px-4 py-2 text-gray-500">Ничего не найдено</p>
            )}
          </div>
        )}
      </div>
    </div>
  );
}
