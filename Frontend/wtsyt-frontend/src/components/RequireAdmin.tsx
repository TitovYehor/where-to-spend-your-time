import { useAuth } from "../contexts/AuthContext";
import { Navigate } from "react-router-dom";
import type { ReactNode } from "react";

type RequireAdminProps = {
  children: ReactNode;
};

export default function RequireAdmin({ children }: RequireAdminProps) {
  const { user, loading } = useAuth();

  if (loading) {
    return <p className="text-center mt-8">Checking permissions...</p>;
  }

  if (!user || user.role !== "Admin") {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
}