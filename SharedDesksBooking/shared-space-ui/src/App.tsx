import { useEffect, useState } from "react";
import './App.css';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import 'bootstrap/dist/css/bootstrap.min.css';

import { Desk, ProfileData, User } from "./types";
import { deskService, reservationService, userService } from "./services/api";

import Navbar from "./components/Navbar";
import IdentificationSection from "./components/IdentificationSection";
import ProfileView from "./components/ProfileView";
import DeskCard from "./components/DeskCard";

function App() {
  // --- STATE MANAGEMENT ---
  const [desks, setDesks] = useState<Desk[]>([]);
  const [selectedDate, setSelectedDate] = useState<Date>(new Date());
  const [user, setUser] = useState<User>({ firstName: "", lastName: "" });

  const [bookingDeskId, setBookingDeskId] = useState<number | null>(null);
  const [startDate, setStartDate] = useState<Date>(new Date());
  const [endDate, setEndDate] = useState<Date | null>(new Date());

  const [view, setView] = useState<"grid" | "profile">("grid");
  const [profileData, setProfileData] = useState<ProfileData | null>(null);

  // --- API CALL HANDLERS ---
  const loadDesks = async (date: Date) => {
    try {
      const formattedDate = date.toLocaleDateString('en-CA');
      const data = await deskService.getDesks(formattedDate);
      setDesks(data);
    } catch (error) {
      console.error("Error: Could not fetch desks.");
    }
  };

  const handleCancel = async (reservationId: number, onlyToday: boolean) => {
    const message = onlyToday
      ? "Are you sure you want to cancel for today only?"
      : "Are you sure you want to cancel the entire reservation range?";

    if (!window.confirm(message)) return;

    try {
      const formattedDate = selectedDate.toLocaleDateString('en-CA');
      await reservationService.cancel(reservationId, onlyToday, formattedDate);
      loadDesks(selectedDate);
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
        startDate: startDate.toLocaleDateString('en-CA'),
        endDate: (endDate || startDate).toLocaleDateString('en-CA')
      });

      setBookingDeskId(null);
      loadDesks(selectedDate);
    } catch (error: any) {
      alert(error.response?.data || "Booking failed");
    }
  };

  const fetchProfile = async () => {
    if (!user.firstName || !user.lastName) {
      alert("Please enter your first and last name to view profile.");
      return;
    }

    try {
      const data = await userService.getProfile(user.firstName, user.lastName);
      setProfileData(data);
      setView("profile");
    } catch (error) {
      console.error("Error fetching profile:", error);
      alert("Failed to fetch profile data.");
    }
  };

  useEffect(() => {
    loadDesks(selectedDate);
  }, [selectedDate]);

  return (
    <div className="container mt-4 pb-5">
      <Navbar
        view={view}
        onViewChange={setView}
        onProfileClick={fetchProfile}
      />

      {view === "grid" ? (
        <div className="animate-fade-in">
          <h1 className="text-center mb-4">Shared Desks Booking</h1>

          <IdentificationSection
            user={user}
            onUserChange={setUser}
          />

          <div className="row justify-content-center mb-5">
            <div className="col-md-4 p-4 shadow-sm bg-light rounded text-center border">
              <label className="form-label fw-bold d-block">Select View Date:</label>
              <DatePicker
                selected={selectedDate}
                onChange={(date: Date | null) => date && setSelectedDate(date)}
                dateFormat="yyyy-MM-dd"
                className="form-control text-center"
              />
            </div>
          </div>

          <div className="row g-4">
            {desks.map((desk) => (
              <DeskCard
                key={desk.id}
                desk={desk}
                user={user}
                bookingDeskId={bookingDeskId}
                startDate={startDate}
                endDate={endDate}
                onCancel={handleCancel}
                onReserve={handleReserve}
                onSetBookingDeskId={setBookingDeskId}
                onSetStartDate={setStartDate}
                onSetEndDate={setEndDate}
              />
            ))}
          </div>
        </div>
      ) : (
        <ProfileView profileData={profileData} />
      )}
    </div>
  );
}

export default App;