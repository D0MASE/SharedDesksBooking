import { useState, useEffect } from "react";
import { Desk, User } from "../types";
import { deskService, reservationService } from "../services/api";

export const useDesks = (initialDate: Date, user: User) => {
    const [desks, setDesks] = useState<Desk[]>([]);
    const [selectedDate, setSelectedDate] = useState<Date>(initialDate);
    const [bookingDeskId, setBookingDeskId] = useState<number | null>(null);
    const [startDate, setStartDate] = useState<Date>(new Date());
    const [endDate, setEndDate] = useState<Date | null>(new Date());
    const [loading, setLoading] = useState(false);

    const loadDesks = async (date: Date) => {
        setLoading(true);
        try {
            const formattedDate = date.toLocaleDateString("en-CA");
            const data = await deskService.getDesks(formattedDate);
            setDesks(data);
        } catch (error) {
            console.error("Error: Could not fetch desks.");
        } finally {
            setLoading(false);
        }
    };

    const handleCancel = async (reservationId: number, onlyToday: boolean) => {
        const message = onlyToday
            ? "Are you sure you want to cancel for today only?"
            : "Are you sure you want to cancel the entire reservation range?";

        if (!window.confirm(message)) return;

        try {
            const formattedDate = selectedDate.toLocaleDateString("en-CA");
            await reservationService.cancel(reservationId, onlyToday, formattedDate);
            await loadDesks(selectedDate);
        } catch (error) {
            console.error("Error cancelling reservation:", error);
            alert("Failed to cancel reservation.");
        }
    };

    const handleReserve = async (deskId: number) => {
        if (!user.firstName || !user.lastName) {
            alert("Please enter your first and last name to make a reservation.");
            return;
        }

        try {
            await reservationService.create({
                deskId: deskId,
                firstName: user.firstName,
                lastName: user.lastName,
                startDate: startDate.toLocaleDateString("en-CA"),
                endDate: (endDate || startDate).toLocaleDateString("en-CA"),
            });

            setBookingDeskId(null);
            await loadDesks(selectedDate);
        } catch (error: any) {
            alert(error.response?.data || "Booking failed");
        }
    };

    useEffect(() => {
        loadDesks(selectedDate);
    }, [selectedDate]);

    return {
        desks,
        loading,
        selectedDate,
        setSelectedDate,
        bookingDeskId,
        setBookingDeskId,
        startDate,
        setStartDate,
        endDate,
        setEndDate,
        handleCancel,
        handleReserve,
        refresh: () => loadDesks(selectedDate),
    };
};
