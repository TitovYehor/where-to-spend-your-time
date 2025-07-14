import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import { login, register } from "../services/authService";
import { getMyProfile } from "../services/userService";

const isValidEmail = (email: string) =>
  /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);

const Register = () => {
  const { setUser } = useAuth();
  const navigate = useNavigate();

  const [displayName, setDisplayName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [errorMessages, setErrorMessages] = useState<string[]>([]);

  const validate = () => {
    if (displayName.trim().length < 2) {
        setError("Display name must be at least 2 characters");
        return false;
    }

    if (!isValidEmail(email)) {
        setError("Please enter a valid email address");
        return false;
    }

    if (password.length < 6) {
        setError("Password must be at least 6 characters");
        return false;
    }

    return true;
    };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setErrorMessages([]);

    if (!validate()) return;

    setLoading(true);
    try {
      await register({displayName, email, password});

      await login({email, password});

      await getMyProfile()
        .then(setUser)
        .catch((e) => console.error('Failed to fetch reviews', e));
      
      navigate("/");
    } catch (err: any) {
      const messages = err?.response?.data;
      setErrorMessages(Array.isArray(messages) ? messages : [messages?.message || "Failed to register"]);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100 px-4">
      <div className="bg-white p-8 rounded-2xl shadow-md w-full max-w-md">
        <h2 className="text-2xl font-bold mb-6 text-center">Register</h2>
        {error && <ul className="mb-4 text-red-500 text-sm list-disc list-inside"><li>{error}</li></ul>}
        {errorMessages.length > 0 && (
        <ul className="mb-4 text-red-500 text-sm list-disc list-inside">
            {errorMessages.map((msg, i) => (
            <li key={i}>{msg}</li>
            ))}
        </ul>
        )}
        <form onSubmit={handleSubmit} className="space-y-4">
          <input
            type="text"
            placeholder="Display Name"
            className="w-full px-4 py-2 border rounded-lg"
            value={displayName}
            onChange={(e) => setDisplayName(e.target.value)}
            required
          />
          <input
            type="email"
            placeholder="Email"
            className="w-full px-4 py-2 border rounded-lg"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
          <input
            type="password"
            placeholder="Password"
            className="w-full px-4 py-2 border rounded-lg"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
          <button
            type="submit"
            className="w-full bg-blue-600 hover:bg-blue-700 text-white py-2 rounded-lg font-semibold"
            disabled={loading}
          >
            {loading ? "Registering..." : "Register"}
          </button>
        </form>
        <p className="mt-4 text-center text-sm">
          Already have an account?{" "}
          <a href="/login" className="text-blue-600 hover:underline">
            Login
          </a>
        </p>
      </div>
    </div>
  );
};

export default Register;