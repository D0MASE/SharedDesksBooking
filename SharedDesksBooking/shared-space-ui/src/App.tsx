import { useState } from "react";
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';

import { ProfileData, User } from "./types";
import { userService } from "./services/api";

import Navbar from "./components/Navbar";
import DesksPage from "./pages/DesksPage";
import ProfilePage from "./pages/ProfilePage";

function App() {
  const [user, setUser] = useState<User>({ firstName: "", lastName: "" });
  const [view, setView] = useState<"grid" | "profile">("grid");
  const [profileData, setProfileData] = useState<ProfileData | null>(null);

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

  return (
    <div className="container mt-4 pb-5">
      <Navbar
        view={view}
        onViewChange={setView}
        onProfileClick={fetchProfile}
      />

      {view === "grid" ? (
        <DesksPage user={user} onUserChange={setUser} />
      ) : (
        <ProfilePage profileData={profileData} />
      )}
    </div>
  );
}

export default App;