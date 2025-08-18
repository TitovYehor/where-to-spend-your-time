import { createContext, useContext, useEffect, useState } from "react";
import type { AuthUser } from "../types/authUser";
import { getMyProfile } from "../services/userService";
import { logout } from "../services/authService";
import { handleApiError } from "../utils/handleApi";

type AuthContextType = {
  user: AuthUser | null;
  loading: boolean;
  setUser: (user: AuthUser | null) => void;
  userLogout: () => void;
  refreshUser: () => void;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchUser = async () => {
      await getMyProfile()
        .then((u) => {
          setUser(u);
        })
        .catch(() => {
          setUser(null);
        })
        .finally(() => setLoading(false));
    };

    fetchUser();
  }, []);


  const refreshUser = async () => {
    await getMyProfile()
      .then((u) => {
        setUser(u);
      })
      .catch(() => {
        setUser(null);
      })
      .finally(() => setLoading(false));
  };

  const userLogout = async () => {
    await logout()
      .catch(handleApiError)
      .finally(() => {
        setUser(null);
        setLoading(false);
      })
  };

  return (
    <AuthContext.Provider value={{ user, loading, setUser, userLogout, refreshUser }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside AuthProvider");
  return ctx;
};