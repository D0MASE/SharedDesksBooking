export const API_BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5143/api";

export const ENDPOINTS = {
    DESKS: "/desks",
    RESERVATIONS: "/reservations",
    PROFILE: "/profile",
} as const;
