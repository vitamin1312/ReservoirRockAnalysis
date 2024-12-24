import React, { useState } from "react";

export interface FieldData {
  id: number;
  name: string;
  description: string;
}

type DropdownProps = {
  options: FieldData[];
  placeholder: string;
  label: string;
  onSelect: (id: number) => void;
};

export default function Dropdown({
  options,
  label,
  placeholder,
  onSelect,
}: DropdownProps) {
  const [isOpen, setIsOpen] = useState<boolean>(false);
  const [selectedOption, setSelectedOption] = useState<string>("");
  const [selectedDescription, setSelectedDescription] = useState<string>("");
  const [searchParam, setSearchParam] = useState<string>("");

  const filteredOptions = options.filter((option) =>
    option.name.toLowerCase().includes(searchParam.toLowerCase())
  );

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchParam(e.target.value);
    setSelectedOption(e.target.value);
    setIsOpen(true);
  };

  const toggleDropdown = () => setIsOpen(!isOpen);

  const handleOptionClick = (option: FieldData) => {
    setSelectedOption(option.name);
    setSelectedDescription(option.description);
    onSelect(option.id);
    setIsOpen(false);
  };

  return (
    <div className="">
      <label htmlFor="dropdown" className="block text-sm font-medium text-gray-900">
        {label}
      </label>

      <input
        id="dropdown"
        type="text"
        value={selectedOption || searchParam}
        onClick={toggleDropdown}
        onChange={handleSearchChange}
        placeholder={placeholder}
        className="mt-1 px-4 py-2 w-full bg-white border border-primary rounded-lg shadow-sm focus:outline-none"
      />

      {isOpen && (
        <div className="absolute z-20 mt-1 bg-white border rounded-lg max-h-60 overflow-y-auto">
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
            <p className="px-4 py-2 text-gray-500">Ничего нет</p>
          )}
        </div>
      )}

      {selectedDescription && (
        <div>
            {selectedDescription}
        </div>
      )}
    </div>
  );
}
