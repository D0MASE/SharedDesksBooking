import axios from "axios";
import { Desk, ProfileData } from "../types";

const API_BASE_URL = "http://localhost:5143/api";

export const deskService = {
    getDesks: async (date: string): Promise<Desk[]> => {
        const response = await axios.get(`${API_BASE_URL}/desks?date=${date}`);
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
        return await axios.post(`${API_BASE_URL}/reservations`, reservation);
    },
    cancel: async (reservationId: number, onlyToday: boolean, date: string) => {
        return await axios.delete(
            `${API_BASE_URL}/reservations/${reservationId}?onlyToday=${onlyToday}&date=${date}`
        );
    },
};

export const userService = {
    getProfile: async (firstName: string, lastName: string): Promise<ProfileData> => {
        const response = await axios.get(
            `${API_BASE_URL}/profile?firstName=${firstName}&lastName=${lastName}`
        );
        return response.data;
    },
};
