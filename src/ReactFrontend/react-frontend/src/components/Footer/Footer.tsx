import React from 'react';

const Footer: React.FC = () => {
  const pageTitle = location.pathname === "/" ? "Главная страница" : "Страница не найдена";
  return (
    <footer className="h-16 p-4 text-gray-900 bg-blue-400 text-xl flex justify-between items-center">
      <div>{pageTitle}</div>
      <div className="font-semibold text-right">РГУ нефти и газа (НИУ) имени И.М. Губкина</div>
    </footer>
  );
};

export default Footer;
