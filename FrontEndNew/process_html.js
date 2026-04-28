const fs = require('fs');
const path = require('path');

const designDir = 'D:\\.NET\\ASPDotNet\\FoodDelivery\\FrontEndNew\\Design_Files\\stitch_quickbites_nocturnal_glass_delivery_DeliveryAgentDashboard';
const appDir = 'D:\\.NET\\ASPDotNet\\FoodDelivery\\FrontEndNew\\src\\app\\features\\delivery';

function processHtml(filepath) {
    let content = fs.readFileSync(filepath, 'utf-8');
    
    // Replace global classes
    content = content.replace(/bg-background/g, 'bg-agent-background');
    content = content.replace(/text-on-background/g, 'text-agent-on-background');
    content = content.replace(/text-primary/g, 'text-agent-primary');
    content = content.replace(/text-on-surface-variant/g, 'text-agent-on-surface-variant');
    
    content = content.replace(/glass-card/g, 'agent-glass-card');
    content = content.replace(/neomorphic-inset/g, 'agent-neomorphic-inset');
    content = content.replace(/neon-glow/g, 'agent-neon-glow');
    content = content.replace(/neon-border/g, 'agent-neon-border');
    content = content.replace(/progress-glow/g, 'agent-progress-glow');
    
    return content;
}

const dashboardHtml = processHtml(path.join(designDir, 'agent_dashboard_desktop', 'code.html'));

const headerMatch = dashboardHtml.match(/<header[\s\S]*?<\/header>/);
const navMatch = dashboardHtml.match(/<nav[\s\S]*?<\/nav>/);

let header = headerMatch ? headerMatch[0] : '';
let nav = navMatch ? navMatch[0] : '';

// Fix links in Nav
nav = nav.replace(/href="#"/g, '');

nav = nav.replace(/<a(.*?)>\s*<span class="material-symbols-outlined">dashboard<\/span>\s*<span>Dashboard<\/span>\s*<\/a>/, 
    '<a$1 routerLink="/delivery/dashboard" routerLinkActive="bg-white/5 text-[#00ff88] shadow-[inset_0_0_10px_rgba(0,255,136,0.1)] border-r-4 border-[#00ff88]" [routerLinkActiveOptions]="{exact: true}">\n<span class="material-symbols-outlined">dashboard</span>\n<span>Dashboard</span>\n</a>');

nav = nav.replace(/<a(.*?)>\s*<span class="material-symbols-outlined">assignment_turned_in<\/span>\s*<span>Assigned Tasks<\/span>\s*<\/a>/, 
    '<a$1 routerLink="/delivery/tasks" routerLinkActive="bg-white/5 text-[#00ff88] shadow-[inset_0_0_10px_rgba(0,255,136,0.1)] border-r-4 border-[#00ff88]">\n<span class="material-symbols-outlined">assignment_turned_in</span>\n<span>Assigned Tasks</span>\n</a>');

nav = nav.replace(/<a(.*?)>\s*<span class="material-symbols-outlined">history<\/span>\s*<span>History<\/span>\s*<\/a>/, 
    '<a$1 routerLink="/delivery/history" routerLinkActive="bg-white/5 text-[#00ff88] shadow-[inset_0_0_10px_rgba(0,255,136,0.1)] border-r-4 border-[#00ff88]">\n<span class="material-symbols-outlined">history</span>\n<span>History</span>\n</a>');

nav = nav.replace(/<a(.*?)>\s*<span class="material-symbols-outlined">account_circle<\/span>\s*<span>Profile<\/span>\s*<\/a>/, 
    '<a$1 routerLink="/delivery/profile" routerLinkActive="bg-white/5 text-[#00ff88] shadow-[inset_0_0_10px_rgba(0,255,136,0.1)] border-r-4 border-[#00ff88]">\n<span class="material-symbols-outlined">account_circle</span>\n<span>Profile</span>\n</a>');

// Remove hardcoded active classes from the links
nav = nav.replace(/bg-white\/5 text-\\[#00ff88\\] shadow-\\[inset_0_0_10px_rgba\\(0,255,136,0\.1\\)\\] border-r-4 border-\\[#00ff88\\] transition-colors duration-300/g, 
    'text-zinc-500 hover:bg-white/5 hover:text-zinc-200 transition-colors duration-300');


const layoutHtml = `<div class="bg-agent-background text-agent-on-background font-body-md text-body-md overflow-x-hidden min-h-screen dark">\n${header}\n${nav}\n<router-outlet></router-outlet>\n</div>`;

fs.writeFileSync(path.join(appDir, 'delivery-layout', 'delivery-layout.html'), layoutHtml, 'utf-8');

const dashboardMainMatch = dashboardHtml.match(/<main[^>]*>([\s\S]*?)<\/main>/);
if (dashboardMainMatch) {
    fs.writeFileSync(path.join(appDir, 'delivery-dashboard', 'delivery-dashboard.html'), 
        '<main class="ml-64 pt-16 p-container-margin">\n' + dashboardMainMatch[1] + '\n</main>', 'utf-8');
}

const profileHtml = processHtml(path.join(designDir, 'agent_profile_desktop', 'code.html'));
const profileMainMatch = profileHtml.match(/<main[^>]*>([\s\S]*?)<\/main>/);
if (profileMainMatch) {
    fs.writeFileSync(path.join(appDir, 'delivery-profile', 'delivery-profile.html'), 
        '<main class="ml-64 pt-16 p-container-margin">\n' + profileMainMatch[1] + '\n</main>', 'utf-8');
}

const tasksHtml = processHtml(path.join(designDir, 'assigned_tasks_desktop', 'code.html'));
const tasksMainMatch = tasksHtml.match(/<main[^>]*>([\s\S]*?)<\/main>/);
if (tasksMainMatch) {
    fs.writeFileSync(path.join(appDir, 'delivery-assigned-tasks', 'delivery-assigned-tasks.html'), 
        '<main class="ml-64 pt-16 p-container-margin">\n' + tasksMainMatch[1] + '\n</main>', 'utf-8');
}

const historyHtml = processHtml(path.join(designDir, 'delivery_history_desktop', 'code.html'));
const historyMainMatch = historyHtml.match(/<main[^>]*>([\s\S]*?)<\/main>/);
if (historyMainMatch) {
    fs.writeFileSync(path.join(appDir, 'delivery-history', 'delivery-history.html'), 
        '<main class="ml-64 pt-16 p-container-margin">\n' + historyMainMatch[1] + '\n</main>', 'utf-8');
}

console.log('Processed all HTML files successfully!');
