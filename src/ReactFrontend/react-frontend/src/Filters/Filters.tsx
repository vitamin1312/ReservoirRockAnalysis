import React from 'react';

interface FiltersProps {
    onToggleView: () => void;
    isTableView: boolean;
}

const Filters: React.FC<FiltersProps> = ({onToggleView, isTableView}) => {
    return (
        <div className='h-full flex flex-col'>
        <div className="p-4 flex flex-row">
            <div className="flex items-center space-x-2">
                <p className="text-gray-900">Таблица</p>
                <input
                    type="checkbox"
                    checked={isTableView}
                    onChange={onToggleView}
                    className="form-checkbox h-5 w-5 text-green-300"
                />
            </div>
        </div>

        <div className='p-4 items-center border-t'>
            <form className="flex flex-col space-y-4">
                <input
                        type="query"
                        placeholder="Поиск"
                        className="p-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-400"
                        required
                    />
            </form>
        </div>

        <div className="p-4 flex flex-row items-center">
            <div className="flex items-center space-x-2">
                <p className="text-gray-900">По возрастанию</p>
                <input
                    type="checkbox"
                    className="form-checkbox h-5 w-5 text-green-300"
                />
            </div>
        </div>
        
        <div className='flex-grow'></div>

        <button
          type="submit"
          className="bg-blue-400 text-white py-3 rounded-md hover:bg-blue-600 p-3"
        >
          Применить
        </button>
        </div>
    )
}

export default Filters;