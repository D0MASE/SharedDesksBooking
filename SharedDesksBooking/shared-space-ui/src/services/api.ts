import axios from "axios";
import { Desk, ProfileData } from "../types";
import { API_BASE_URL, ENDPOINTS } from "../constants/apiEndpoints";

export const deskService = {
    getDesks: async (date: string): Promise<Desk[]> => {
        const response = await axios.get(`${API_BASE_URL}${ENDPOINTS.DESKS}?date=${date}`);
        return response.data;
    },
};

export const reservationService = {
    create: async (reservation: {
        deskId: number;
        firstName: string;
        lastName: string;
        startDate: string;
        endDate: string;
    }) => {
        return await axios.post(`${API_BASE_URL}${ENDPOINTS.RESERVATIONS}`, reservation);
    },
    cancel: async (reservationId: number, onlyToday: boolean, date: string) => {
        return await axios.delete(
            `${API_BASE_URL}${ENDPOINTS.RESERVATIONS}/${reservationId}?onlyToday=${onlyToday}&date=${date}`
        );
    },
};

export const userService = {
    getProfile: async (firstName: string, lastName: string): Promise<ProfileData> => {
        const response = await axios.get(
            `${API_BASE_URL}${ENDPOINTS.PROFILE}?firstName=${firstName}&lastName=${lastName}`
        );
        return response.data;
    },
};
