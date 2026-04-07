import { useEffect, useState } from "react";
import { Link, useNavigate, useSearchParams } from "react-router-dom";
import { Lock, CheckCircle } from "lucide-react";
import Alert from "../components/common/Alerts";
import { resetPassword } from "../services/authService";
import { handleApiError } from "../utils/handleApi";
import { extractProblemDetailsError } from "../utils/extractProblemDetailsError";

export default function ResetPasswordConfirm() {
  const navigate = useNavigate();
  const [params] = useSearchParams();

  const email = params.get("email") ?? "";
  const token = params.get("token") ?? "";

  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState("");
  const [error, setError] = useState<string | string[]>("");

  const validate = () => {
    const newErrors: string[] = [];

    if (newPassword.length < 6) {
      newErrors.push("Password must be at least 6 characters");
    }

    if (newPassword !== confirmPassword) {
      newErrors.push("Passwords do not match");
    }

    setError(newErrors);
    return newErrors.length === 0;
  };

  useEffect(() => {
    if (!email || !token) {
      setError("Invalid password reset link");
    }
  }, [email, token]);

  const handleSubmit = async (e: any) => {
    e.preventDefault();
    setError("");
    setSuccess("");

    if (!validate()) return;

    setLoading(true);
    try {
      await resetPassword({ email, token, newPassword });
      setSuccess("Password successfully reset! Redirecting to login...");

      setTimeout(() => navigate("/login"), 2000);
    } catch (err: any) {
      handleApiError(err);
      setError(extractProblemDetailsError(err));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100 px-4">
      <div className="bg-white p-8 rounded-2xl shadow-md w-full max-w-md">
        <h2 className="text-2xl font-bold mb-6 text-center">
          Set New Password
        </h2>

        <Alert type="success" message={success} onClose={() => setSuccess("")} />
        <Alert type="error" message={error} onClose={() => setError("")} />

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              New Password
            </label>
            <div className="flex items-center border rounded-lg px-3 focus-within:ring-2 focus-within:ring-blue-500">
              <Lock className="w-4 h-4 text-gray-400 mr-2" />
              <input
                type="password"
                className="w-full py-2 focus:outline-none"
                placeholder="********"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                required
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Confirm Password
            </label>
            <div className="flex items-center border rounded-lg px-3 focus-within:ring-2 focus-within:ring-blue-500">
              <Lock className="w-4 h-4 text-gray-400 mr-2" />
              <input
                type="password"
                className="w-full py-2 focus:outline-none"
                placeholder="********"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                required
              />
            </div>
          </div>

          <button
            type="submit"
            disabled={loading || !email || !token}
            className="w-full flex items-center justify-center gap-2 bg-blue-600 hover:bg-blue-700 text-white py-2 rounded-lg font-semibold transition"
          >
            <CheckCircle className="w-4 h-4" />
            {loading ? "Saving..." : "Reset Password"}
          </button>
        </form>

        <p className="mt-4 text-center text-sm">
          Back to{" "}
          <Link to="/login" className="text-blue-600 hover:underline">
            login
          </Link>
        </p>
      </div>
    </div>
  );
}