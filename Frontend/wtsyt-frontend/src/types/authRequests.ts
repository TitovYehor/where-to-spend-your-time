export type LoginRequest = {
    email: string;
    password: string;
};

export type RegisterRequest = {
    email: string;
    password: string;
    displayName: string;
};

export type PasswordResetRequest = {
    email: string;
};

export type PasswordResetConfirm = {
    email: string;
    token: string;
    newPassword: string;
};