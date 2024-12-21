import React from 'react';

interface FiltersProps {
    onToggleView: () => void;
    isTableView: boolean;
}

const Filters: React.FC<FiltersProps> = ({onToggleView, isTableView}) => {
    return (
        <div className="p-4 flex flex-row items-center">
        <div className="flex items-center space-x-2">
            <p className="text-gray-900 font-medium">Таблица</p>
            <input
            type="checkbox"
            checked={isTableView}
            onChange={onToggleView}
            className="form-checkbox h-5 w-5 text-green-300"
            />
        </div>
        </div>
    )
}

export default Filters;