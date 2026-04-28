import os
import re

design_dir = r'D:\.NET\ASPDotNet\FoodDelivery\FrontEndNew\Design_Files\stitch_quickbites_nocturnal_glass_delivery_DeliveryAgentDashboard'
app_dir = r'D:\.NET\ASPDotNet\FoodDelivery\FrontEndNew\src\app\features\delivery'

def process_html(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Replace global classes
    content = content.replace('bg-background', 'bg-agent-background')
    content = content.replace('text-on-background', 'text-agent-on-background')
    content = content.replace('text-primary', 'text-agent-primary')
    content = content.replace('text-on-surface-variant', 'text-agent-on-surface-variant')
    
    content = content.replace('glass-card', 'agent-glass-card')
    content = content.replace('neomorphic-inset', 'agent-neomorphic-inset')
    content = content.replace('neon-glow', 'agent-neon-glow')
    content = content.replace('neon-border', 'agent-neon-border')
    content = content.replace('progress-glow', 'agent-progress-glow')
    
    return content

# Extract Layout
dashboard_html = process_html(os.path.join(design_dir, 'agent_dashboard_desktop', 'code.html'))

# Parse Header and Nav out of dashboard_html
header_match = re.search(r'<header.*?</header>', dashboard_html, re.DOTALL)
nav_match = re.search(r'<nav.*?</nav>', dashboard_html, re.DOTALL)

header = header_match.group(0) if header_match else ''
nav = nav_match.group(0) if nav_match else ''

# Fix links in Nav
nav = nav.replace('href="#"', '')
# Let's manually replace the links to use routerLink
nav = re.sub(r'<a(.*?)>\s*<span class="material-symbols-outlined">dashboard</span>\s*<span>Dashboard</span>\s*</a>', r'<a\1 routerLink="/delivery/dashboard" routerLinkActive="bg-white/5 text-[#00ff88] shadow-[inset_0_0_10px_rgba(0,255,136,0.1)] border-r-4 border-[#00ff88]" [routerLinkActiveOptions]="{exact: true}">\n<span class="material-symbols-outlined">dashboard</span>\n<span>Dashboard</span>\n</a>', nav)

nav = re.sub(r'<a(.*?)>\s*<span class="material-symbols-outlined">assignment_turned_in</span>\s*<span>Assigned Tasks</span>\s*</a>', r'<a\1 routerLink="/delivery/tasks" routerLinkActive="bg-white/5 text-[#00ff88] shadow-[inset_0_0_10px_rgba(0,255,136,0.1)] border-r-4 border-[#00ff88]">\n<span class="material-symbols-outlined">assignment_turned_in</span>\n<span>Assigned Tasks</span>\n</a>', nav)

nav = re.sub(r'<a(.*?)>\s*<span class="material-symbols-outlined">history</span>\s*<span>History</span>\s*</a>', r'<a\1 routerLink="/delivery/history" routerLinkActive="bg-white/5 text-[#00ff88] shadow-[inset_0_0_10px_rgba(0,255,136,0.1)] border-r-4 border-[#00ff88]">\n<span class="material-symbols-outlined">history</span>\n<span>History</span>\n</a>', nav)

nav = re.sub(r'<a(.*?)>\s*<span class="material-symbols-outlined">account_circle</span>\s*<span>Profile</span>\s*</a>', r'<a\1 routerLink="/delivery/profile" routerLinkActive="bg-white/5 text-[#00ff88] shadow-[inset_0_0_10px_rgba(0,255,136,0.1)] border-r-4 border-[#00ff88]">\n<span class="material-symbols-outlined">account_circle</span>\n<span>Profile</span>\n</a>', nav)

# Remove hardcoded active classes from the links
nav = nav.replace('bg-white/5 text-[#00ff88] shadow-[inset_0_0_10px_rgba(0,255,136,0.1)] border-r-4 border-[#00ff88] transition-colors duration-300', 'text-zinc-500 hover:bg-white/5 hover:text-zinc-200 transition-colors duration-300')


layout_html = f'''<div class="bg-agent-background text-agent-on-background font-body-md text-body-md overflow-x-hidden min-h-screen dark">
{header}
{nav}
<router-outlet></router-outlet>
</div>'''

with open(os.path.join(app_dir, 'delivery-layout', 'delivery-layout.html'), 'w', encoding='utf-8') as f:
    f.write(layout_html)

# Extract Main content for Dashboard
main_match = re.search(r'<main.*?>(.*?)</main>', dashboard_html, re.DOTALL)
if main_match:
    with open(os.path.join(app_dir, 'delivery-dashboard', 'delivery-dashboard.html'), 'w', encoding='utf-8') as f:
        # wrap in main tag to keep the layout correct
        f.write('<main class="ml-64 pt-16 p-container-margin">\n' + main_match.group(1) + '\n</main>')

# Process Profile
profile_html = process_html(os.path.join(design_dir, 'agent_profile_desktop', 'code.html'))
main_match = re.search(r'<main.*?>(.*?)</main>', profile_html, re.DOTALL)
if main_match:
    with open(os.path.join(app_dir, 'delivery-profile', 'delivery-profile.html'), 'w', encoding='utf-8') as f:
        f.write('<main class="ml-64 pt-16 p-container-margin">\n' + main_match.group(1) + '\n</main>')

# Process Tasks
tasks_html = process_html(os.path.join(design_dir, 'assigned_tasks_desktop', 'code.html'))
main_match = re.search(r'<main.*?>(.*?)</main>', tasks_html, re.DOTALL)
if main_match:
    with open(os.path.join(app_dir, 'delivery-assigned-tasks', 'delivery-assigned-tasks.html'), 'w', encoding='utf-8') as f:
        f.write('<main class="ml-64 pt-16 p-container-margin">\n' + main_match.group(1) + '\n</main>')

# Process History
history_html = process_html(os.path.join(design_dir, 'delivery_history_desktop', 'code.html'))
main_match = re.search(r'<main.*?>(.*?)</main>', history_html, re.DOTALL)
if main_match:
    with open(os.path.join(app_dir, 'delivery-history', 'delivery-history.html'), 'w', encoding='utf-8') as f:
        f.write('<main class="ml-64 pt-16 p-container-margin">\n' + main_match.group(1) + '\n</main>')

print('Processed all HTML files successfully!')
