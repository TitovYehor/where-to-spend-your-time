import { useState } from "react";
import { Link } from "react-router-dom";
import { Mail, Send } from "lucide-react";
import Alert from "../components/common/Alerts";
import { requestPasswordReset } from "../services/authService";
import { handleApiError } from "../utils/handleApi";
import { extractProblemDetailsError } from "../utils/extractProblemDetailsError";

export default function ResetPassword() {
  const [email, setEmail] = useState("");
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState("");
  const [error, setError] = useState<string | string[]>("");

  const handleSubmit = async (e: any) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    setSuccess("");

    try {
      await requestPasswordReset({ email });
      setSuccess("If this email is registered, a reset link has been sent");
      setEmail("");
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
          Reset Password
        </h2>

        <Alert type="success" message={success} onClose={() => setSuccess("")} />
        <Alert type="error" message={error} onClose={() => setError("")} />

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label
              htmlFor="email"
              className="block text-sm font-medium text-gray-700 mb-1"
            >
              Email
            </label>
            <div className="flex items-center border rounded-lg px-3 focus-within:ring-2 focus-within:ring-blue-500">
              <Mail className="w-4 h-4 text-gray-400 mr-2" />
              <input
                id="email"
                type="email"
                className="w-full py-2 focus:outline-none"
                placeholder="you@example.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
          </div>

          <button
            type="submit"
            className="w-full flex items-center justify-center gap-2 bg-blue-600 hover:bg-blue-700 text-white py-2 rounded-lg font-semibold transition"
            disabled={loading}
          >
            <Send className="w-4 h-4" />
            {loading ? "Sending..." : "Send Reset Email"}
          </button>
        </form>

        <p className="mt-4 text-center text-sm">
          Remember your password?{" "}
          <Link to="/login" className="text-blue-600 hover:underline">
            Back to login
          </Link>
        </p>
      </div>
    </div>
  );
}