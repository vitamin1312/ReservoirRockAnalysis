import React, { useState } from "react";
import Modal from "react-modal";

interface AuthModalProps {
  isOpen: boolean;
  onClose: () => void;
}

const AuthModal: React.FC<AuthModalProps> = ({ isOpen, onClose }) => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log({ email, password });
    onClose();
  };

  return (
    <Modal
      isOpen={isOpen}
      onRequestClose={onClose}
      className="bg-white p-6 rounded-lg shadow-lg max-w-md mx-auto mt-20"
      overlayClassName="fixed inset-0 bg-gray-900 bg-opacity-75 flex justify-center items-center"
    >
      <h2 className="text-2xl font-bold mb-4">Авторизация</h2>
      <form onSubmit={handleSubmit} className="flex flex-col space-y-4">
        <input
          type="login"
          placeholder="Логин"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="p-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-400"
          required
        />
        <input
          type="password"
          placeholder="Пароль"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className="p-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-400"
          required
        />
        <button
          type="submit"
          className="bg-blue-400 text-white py-2 rounded-md hover:bg-blue-400"
        >
          Войти
        </button>
      </form>
      <button
        onClick={onClose}
        className="mt-4 text-gray-500 hover:underline"
      >
        Закрыть
      </button>
    </Modal>
  );
};

export default AuthModal;