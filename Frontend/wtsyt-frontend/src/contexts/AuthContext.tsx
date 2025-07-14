import { createContext, useContext, useEffect, useState } from "react";
import type { AuthUser } from "../types/authUser";
import { getMyProfile } from "../services/userService";
import { logout } from "../services/authService";

type AuthContextType = {
  user: AuthUser | null;
  setUser: (user: AuthUser | null) => void;
  userLogout: () => void;
  refreshUser: () => void;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [user, setUser] = useState<AuthUser | null>(null);

  useEffect(() => {
    const fetchUser = async () => {
      await getMyProfile()
        .then(setUser)
        .catch(() => setUser(null))
    };

    fetchUser();
  }, []);


  const refreshUser = async () => {
    await getMyProfile()
      .then(setUser)
      .catch(() => setUser(null))
  };

  const userLogout = async () => {
    await logout()
      .catch((e) => console.error("Logout failed", e))
      .finally(() => setUser(null))
  };

  return (
    <AuthContext.Provider value={{ user, setUser, userLogout, refreshUser }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside AuthProvider");
  return ctx;
};