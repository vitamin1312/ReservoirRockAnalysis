import React from 'react';
import { useLocation } from 'react-router-dom';

const pages: Record<string, string> = {
  "/": "Главная страница",
  "/upload": "Загрузить",
  "/about": "О приложении",
  "/editor/:id": "Редактирование изображения"
};

// Функция для сопоставления пути с параметром
function matchPath(pathname: string): string {
  if (pathname.startsWith("/editor/")) {
    return "/editor/:id";
  }
  return pathname;
}

const Footer: React.FC = () => {
  const location = useLocation();
  const currentPath = matchPath(location.pathname);
  const pageTitle = pages[currentPath] || "Страница";

  return (
    <footer className="w-full shrink-0 h-16 p-4 text-gray-900 bg-blue-400 text-xl flex justify-between items-center">
      <div>{pageTitle}</div>
      <div className="font-semibold text-right">РГУ нефти и газа (НИУ) имени И.М. Губкина</div>
    </footer>
  );
};

export default Footer;