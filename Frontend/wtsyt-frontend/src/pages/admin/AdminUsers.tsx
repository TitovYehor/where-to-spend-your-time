import { useEffect, useRef, useState } from "react";
import { deleteUser, getPagedUsers, getRoles, updateUserRole } from "../../services/userService.ts";
import type { AuthUser } from "../../types/authUser.ts";
import { handleApiError } from "../../utils/handleApi";
import Select from "react-select";
import { useAuth } from "../../contexts/AuthContext.tsx";
import type { UserPagedResult } from "../../types/pagination/pagedResult.ts";
import { Users as UsersIcon, Search, ChevronLeft, ChevronRight } from "lucide-react";
import UserAdminCard from "../../components/users/UserAdminCard.tsx";
import Alert from "../../components/common/Alerts.tsx";

export default function AdminUsers() {
  const { user } = useAuth();

  const [users, setUsers] = useState<AuthUser[]>([]);
  const [roles, setRoles] = useState<string[]>([]);
  const [totalCount, setTotalCount] = useState(0);

  const [loading, setLoading] = useState(true);

  const [role, setRole] = useState<string | null>(null);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [roleFilter, setRoleFilter] = useState<string>(""); 

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize] = useState(5);

  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  const formRef = useRef<HTMLFormElement>(null);

  const rolesOptions = [
    { value: "", label: "All Roles" },
    ...roles.map((role) => ({ value: role, label: role })),
  ];

  const fetchData = async (signal?: AbortSignal) => {
    try {
      setLoading(true);
      const data: UserPagedResult = await getPagedUsers({
        search,
        role: roleFilter || undefined,
        page,
        pageSize
      }, signal);
      setUsers(data.items);
      setTotalCount(data.totalCount);
    } catch (err) {
      if (!signal?.aborted) {
        handleApiError(err);
        setError("Failed to load data");
      }
    } finally {
      if (!signal?.aborted) {
        setLoading(false);
      }
    }
  };

  useEffect(() => {
    const controller = new AbortController();

    fetchData(controller.signal);

    return () => controller.abort();
  }, [search, page, pageSize, roleFilter]);

  useEffect(() => {
    const controller = new AbortController();

    const fetchRoles = async () => {
      try
      {
        const roles = await getRoles(controller.signal);
        setRoles(roles);
      } catch (e) {
        if (!controller.signal.aborted) {
          handleApiError(e);
        }
      }
    };
    
    fetchRoles();

    return () => controller.abort();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (role == null) {
      setError("Role can't be empty");
      return;
    }

    try {
      if (editingId !== null) {
        await updateUserRole(editingId, { Role: role });
        fetchData();
        setEditingId(null);
        setMessage("User role updated");
      }
      setRole("");
      fetchData();
      setError("");
    } catch (err) {
      handleApiError(err);
      setError("Failed to update user role");
      setMessage("");
    }
  };

  const handleEdit = (user: AuthUser) => {
    setEditingId(user.id);
    setRole(user.role);
    
    setTimeout(() => {
        formRef.current?.scrollIntoView({ behavior: "smooth", block: "center" });
    }, 0);
    
    setError("");
    setMessage("");
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this user?")) return;

    try {
      await deleteUser(id);
      fetchData();
      setError("");
      setMessage("User deleted");
      setEditingId(null);
    } catch (err) {
      handleApiError(err);
      setError("Failed to delete user");
      setMessage("");
    }
  };

  const totalPages = Math.ceil(totalCount / pageSize);

  return (
    <section
      aria-labelledby="manage-users-heading"
      className="max-w-4xl mx-auto p-8 bg-white/80 backdrop-blur-md rounded-2xl shadow-xl"
    >
      <h1 className="text-2xl font-bold mb-6 flex items-center gap-2">
        <UsersIcon className="w-6 h-6 text-violet-600" />
        Manage Users
      </h1>

      <Alert type="success" message={message} onClose={() => setMessage("")} />
      <Alert type="error" message={error} onClose={() => setError("")} />
      
      {editingId && (
        <form ref={formRef} onSubmit={handleSubmit} className="mb-6 space-y-4">
          <div>
            <label htmlFor="userName" className="block text-sm font-medium text-black mb-1">
              User name
            </label>
            <input
              id="userName"
              className="w-full px-4 py-2 border rounded"
              value={users.find(u => u.id == editingId)?.displayName}
              readOnly
            />
          </div>
          <div>
            <label htmlFor="userRole" className="block text-sm font-medium text-black mb-1">
              User role
            </label>
            <Select
              id="userRole"
              options={rolesOptions}
              value={
                rolesOptions.find(
                    (opt) => opt.value === (role ?? "")
                ) || rolesOptions[0]
              }
              onChange={(option) => {
                setRole(option?.value ?? "");
              }}
              classNamePrefix="react-select"
            />
          </div>
          <div className="flex items-center gap-4">
          <button
            type="submit"
            className="bg-blue-600 hover:bg-blue-700 text-white py-2 px-4 rounded transition"
          >
            Update user
          </button>

          <button
            type="button"
            className="text-gray-500 text-sm underline hover:text-gray-700 transition"
            onClick={() => {
                setEditingId(null);
                setRole("");
                setError("");
            }}
            >
            Cancel
          </button>
          </div>
        </form>
      )}

      <div className="flex flex-col sm:flex-row sm:items-end sm:justify-between gap-4 mb-6">
        <div className="flex-1">
          <label htmlFor="search" className="block text-sm font-medium text-black mb-1">
            Search
          </label>
          <div className="relative">
            <Search className="absolute left-3 top-2.5 w-4 h-4 text-gray-400" />
            <input
              id="search"
              type="text"
              placeholder="Search users..."
              value={search}
              onChange={(e) => {
                setSearch(e.target.value);
                setPage(1);
              }}
              className="w-full border border-gray-300 pl-10 px-4 py-1.5 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
        </div>

        <div className="flex-1">
          <label htmlFor="roleFilter" className="block text-sm font-medium text-black mb-1">
            Filter by role
          </label>
          <Select
            id="roleFilter"
            options={rolesOptions}
            value={rolesOptions.find(opt => opt.value === roleFilter) || rolesOptions[0]}
            onChange={(option) => {
              setRoleFilter(option?.value ?? "");
              setPage(1);
            }}
            classNamePrefix="react-select"
          />
        </div>
      </div>

      {loading ? (
        <p className="text-gray-500 text-center">Loading...</p>
      ) : users.length === 0 ? (
        <p className="text-gray-600 text-center">No users found</p>
      ) : (
        <>
          <ul className="space-y-4">
            {users.map((u) => {
              return (
                <UserAdminCard
                  key={u.id}
                  user={user}
                  displayUser={u}
                  onEdit={handleEdit}
                  onDelete={handleDelete}
                />
              );  
            })}
          </ul>

          <div className="flex justify-center items-center gap-3 mt-6">
            <button
              disabled={page === 1}
              onClick={() => setPage((p) => Math.max(p - 1, 1))}
              className="p-2 rounded bg-gray-200 disabled:opacity-50"
              aria-label="Previous page"
            >
              <ChevronLeft className="w-5 h-5" />
            </button>

            <span>
              Page {page} of {totalPages}
            </span>

            <button
              disabled={page === totalPages}
              onClick={() => setPage((p) => Math.min(p + 1, totalPages))}
              className="p-2 rounded bg-gray-200 disabled:opacity-50"
              aria-label="Next page"
            >
              <ChevronRight className="w-5 h-5" />
            </button>
          </div>
        </>
      )}
    </section>
  );
}