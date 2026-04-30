# Admin Components - Light Theme Conversion

## Status: IN PROGRESS

### Completed:
1. ✅ Admin Sidebar - Converted to light theme
2. ✅ Dashboard - Converted to light theme  
3. ✅ Order Management (partial) - Main table converted

### Remaining:
- Order Management Modal
- Pending Approvals
- Reports
- Restaurant Management
- User Management (including modal)

### Design System:
- Background: `#f8f9fa`
- Cards: `white` with `#e5e7eb` borders
- Primary: `#ff6b35` (orange) - replacing emerald green
- Text: `#1a1a1a` (dark), `#6b7280` (gray), `#9ca3af` (light gray)
- Status badges: Light backgrounds (green-50, blue-50, yellow-50, red-50)

### Changes Made:
- Removed dark gradients (`bg-gradient-to-br from-[#0a0a0a]`)
- Removed emerald green (`emerald-400`) → replaced with orange (`#ff6b35`)
- Removed glass-card effects → replaced with white cards
- Updated all text colors for light theme
- Updated borders from `border-white/10` to `border-[#e5e7eb]`
