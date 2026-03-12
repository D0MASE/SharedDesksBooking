import { useState } from "react";
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';

import { ProfileData, User } from "./types";
import { userService } from "./services/api";

import Navbar from "./components/Navbar";
import DesksPage from "./pages/DesksPage";
import ProfilePage from "./pages/ProfilePage";

import { Routes, Route, useNavigate, useLocation } from "react-router-dom";

function App() {
  const [user, setUser] = useState<User>({ firstName: "", lastName: "" });
  const [profileData, setProfileData] = useState<ProfileData | null>(null);
  const navigate = useNavigate();
  const location = useLocation();

  const fetchProfile = async () => {
    if (!user.firstName || !user.lastName) {
      alert("Please enter your first and last name to view profile.");
      return;
    }

    try {
      const data = await userService.getProfile(user.firstName, user.lastName);
      setProfileData(data);
      navigate("/profile");
    } catch (error) {
      console.error("Error fetching profile:", error);
      alert("Failed to fetch profile data.");
    }
  };

  const currentView = location.pathname === "/profile" ? "profile" : "grid";

  return (
    <div className="container mt-4 pb-5">
      <Navbar
        view={currentView}
        onProfileClick={fetchProfile}
      />

      <Routes>
        <Route path="/" element={<DesksPage user={user} onUserChange={setUser} />} />
        <Route path="/profile" element={<ProfilePage profileData={profileData} />} />
      </Routes>
    </div>
  );
}

export default App;