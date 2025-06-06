import React, { useState } from "react";
import Modal from "react-modal";
import { authUser } from "../../RestAPI/RestAPI";

interface AuthModalProps {
  isOpen: boolean;
  onClose: () => void;
}

const AuthModal: React.FC<AuthModalProps> = ({ isOpen, onClose }) => {
  const [login, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [authError, setAuthError] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loginError, setLoginError] = useState<string | null>(null);
  const [passwordError, setPasswordError] = useState<string | null>(null);

  const validateForm = () => {
    let valid = true;

    // Проверка длины логина
    if (login.length < 4) {
      setLoginError("Логин должен быть не короче 4 символов.");
      valid = false;
    } else {
      setLoginError(null);
    }

    // Проверка длины пароля
    if (password.length < 4) {
      setPasswordError("Пароль должен быть не короче 4 символов.");
      valid = false;
    } else {
      setPasswordError(null);
    }

    return valid;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setAuthError("");

    // Проверка валидации
    if (!validateForm()) {
      return; // Прерываем отправку формы, если валидация не пройдена
    }

    try {
      const token = await authUser(login, password);
      if (token) {
        localStorage.setItem("jwtToken", token);
        setAuthError('');
        onClose();
        window.location.reload();
      } else {
        setAuthError('Неверный логин или пароль');
        console.error("Ошибка авторизации:", error);
      }
    } catch (err: any) {
      setAuthError('Неверный логин или пароль');
      setError(err.response?.data?.message || "Ошибка авторизации");
      console.error("Ошибка авторизации:", error);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onRequestClose={onClose}
      className="bg-white p-6 rounded-lg shadow-lg max-w-md mx-auto mt-20 w-1/4"
      overlayClassName="fixed inset-0 bg-gray-900 bg-opacity-75 flex justify-center items-center"
    >
      <h2 className="text-2xl font-bold mb-4">Авторизация</h2>
      <form onSubmit={handleSubmit} className="flex flex-col space-y-4">
        <input
          type="text"
          placeholder="Логин"
          value={login}
          onChange={(e) => setEmail(e.target.value)}
          className="p-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-600"
        />
        {loginError && <p className="text-red-500 text-sm">{loginError}</p>}

        <input
          type="password"
          placeholder="Пароль"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className="p-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-600"
        />
        {passwordError && <p className="text-red-500 text-sm">{passwordError}</p>}

        <button
          type="submit"
          className="bg-blue-400 text-white py-2 rounded-md hover:bg-blue-600"
        >
          Войти
        </button>
      </form>
      <p className="text-red-500 m-2">{authError}</p>
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
