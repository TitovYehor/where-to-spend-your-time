import React from "react";
import { X } from "lucide-react";

interface AlertProps {
  type?: "success" | "error";
  message: string;
  onClose: () => void;
}

const Alert: React.FC<AlertProps> = ({ type = "success", message, onClose }) => {
  if (!message) return null;

  const styles = {
    success: "bg-green-50 border-green-300 text-green-800",
    error: "bg-red-50 border-red-300 text-red-800",
  }[type];

  const buttonStyles = {
    success: "text-green-600 hover:text-green-800",
    error: "text-red-600 hover:text-red-800",
  }[type];

  return (
    <div
      className={`flex items-center justify-between border text-sm px-4 py-2 rounded-md shadow-sm mb-3 ${styles}`}
      role="alert"
    >
      <div>
        <span>{message}</span>
      </div>
      <button
        onClick={onClose}
        className={`${buttonStyles} flex items-center gap-1 font-medium`}
      >
        <X size={14} />
      </button>
    </div>
  );
};

export default Alert;